using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            /*--- we even added configuration for age here
              --- the reason why we removed the configuration
              --- from AppUser entity because 
              --- for calculating the age
              --- we used to fetch all the columns from app user
              --- but fetching all columns is of no use
              --- as it is not populated to the client side as some of them are not required
              --- so we are setting up the configuration here
            */
            //individual mapping for individual property
            //that auto mapper doesnt understand by default
            CreateMap<AppUser,MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo,PhotoDto>();
        }
    }
}