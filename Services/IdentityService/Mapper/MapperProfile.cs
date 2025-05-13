using AutoMapper;
using System;
using TalentHire.Services.IdentityService.DTOs;
using TalentHire.Services.IdentityService.Models;
public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        CreateMap<Credentials, CredentialsDto>();
        CreateMap<CredentialsDto, Credentials>();

    }
}