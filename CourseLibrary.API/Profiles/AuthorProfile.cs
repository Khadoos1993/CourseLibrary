using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.Helpers;

namespace CourseLibrary.API.Profiles
{
    public class AuthorProfile: Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.Name, act => act.MapFrom(src => $"{src.FirstName}-{src.LastName}"))
                .ForMember(dest => dest.Age, act => act.MapFrom(src => src.DateOfBirth.GetCurrentAge()));
            CreateMap<AuthorForUpdate, Author>()
                .ForMember(dest => dest.FirstName, act => act.MapFrom(src => src.Name.Split('-', StringSplitOptions.RemoveEmptyEntries)[0]))
                .ForMember(dest => dest.LastName, act => act.MapFrom(src => src.Name.Split('-', StringSplitOptions.RemoveEmptyEntries)[1]))
                .ForMember(dest => dest.DateOfBirth, act => act.MapFrom(src => new DateTime(DateTime.UtcNow.Year-src.Age, 1, 1)));
            CreateMap<AuthorDtoForCreation, Author>();
            CreateMap<Author,AuthorDtoForCreation>();

            CreateMap<Author, AuthorFullDto>()
                .ForMember(dest => dest.DateOfBirth, act =>
                    act.MapFrom((src, dest) =>
                    {
                        return DateTimeOffset.TryParse(src.DateOfBirth, out DateTimeOffset date) ? date : DateTimeOffset.Now;
                    }));

        }


    }
}
