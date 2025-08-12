using System.Net.Http;
using System.Threading.Tasks;
namespace TalentHire.Services.ApplicationsService.Services
{
    public class JobServiceClient : IJobServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> IsJobOwnedByUserAsync(int jobId)
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(accessToken))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Get, $"http://job-service:8081/api/jobs/{jobId}/");
            Console.WriteLine(request.RequestUri);
            Console.WriteLine($"Access Token: {accessToken}");
            request.Headers.Add("Authorization", accessToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            return bool.TryParse(content, out bool isOwner) && isOwner;
        }
    }
}