using AutoMapper;
using BaseLibrary;
using Repository.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NovelOnline.App.AutoMapper.Profiles
{
    public class WebsiteNovelAndNVNovel : Profile
    {
        public WebsiteNovelAndNVNovel()
        {
            CreateMap<NVNovel, WebsiteNovel>()
                .ForMember(d => d.OriginLink, opt => opt.MapFrom(s => s.Link))
                .ForMember(d => d.State, opt => opt.Ignore())
                .ForMember(d => d.CreateDate, opt => opt.Ignore());
        }
    }
}
