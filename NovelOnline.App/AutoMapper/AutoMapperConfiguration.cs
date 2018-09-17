using AutoMapper;
using NovelOnline.App.AutoMapper.Profiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NovelOnline.App.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<WebsiteNovelAndNVNovel>();
            });
            Mapper.AssertConfigurationIsValid();
        }
    }
}
