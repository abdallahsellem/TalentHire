namespace TalentHire.Services.ApplicationsService.Services
{


    public interface IJobServiceClient
    {
        Task<bool> IsJobOwnedByUserAsync(int jobId);
    }
}
