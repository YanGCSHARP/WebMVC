using AutoMapper;
using WebMVC.Dtos;
using WebMVC.Models;

namespace WebMVC.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>();
    }
}