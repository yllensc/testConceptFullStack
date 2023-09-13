using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Domine.Entities;

namespace API.Profiles;
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<Client,ClientDto>()
                .ReverseMap();
        }
    }