using AutoMapper;
using System;
using TalentHire.Services.JobService.Models;
using TalentHire.Services.JobService.DTOs;
namespace TalentHire.Services.JobService.Mapper
{


    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Job, JobDto>();
            CreateMap<JobDto, Job>();

        }
    }
};