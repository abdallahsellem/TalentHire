using AutoMapper;
using TalentHire.Services.ApplicationsService.Models;
using TalentHire.Services.ApplicationsService.DTOs;

namespace TalentHire.Services.ApplicationsService.Mapper
{



    public class ApplicationMapperProfile : Profile
    {
        public ApplicationMapperProfile()
        {
            // Application entity to DTO mappings
            CreateMap<Application, ApplicationDto>();

            // DTO to Application entity mappings
            CreateMap<CreateApplicationDto, Application>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewerNotes, opt => opt.Ignore());

            CreateMap<ApplicationDto, Application>();

            // Update DTO mappings
            CreateMap<UpdateApplicationStatusDto, Application>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.JobId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicantName, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicantEmail, opt => opt.Ignore())
                .ForMember(dest => dest.ResumeUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CoverLetter, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationDate, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewedDate, opt => opt.Ignore());
        }
    }

}