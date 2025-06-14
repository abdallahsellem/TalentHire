using AutoMapper;
using System;
using TalentHire.Services.ApplicationsService.Models;
using TalentHire.Services.ApplicationsService.DTOs;
namespace TalentHire.Services.JobService.Mapper
{


    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Application, ApplicationDto>();
            CreateMap<ApplicationDto, Application>();

        }
    }
};