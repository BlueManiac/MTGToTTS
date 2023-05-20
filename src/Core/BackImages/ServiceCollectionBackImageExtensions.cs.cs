using System;
using System.Collections.Generic;
using Core;
using Microsoft.Extensions.DependencyInjection;

namespace Core.BackImages
{
    public static class ServiceCollectionBackImageExtensions
    {
        public static IServiceCollection RegisterBackImageResolvers(this IServiceCollection collection)
        {
            return collection.AddSingleton(Create);

            static IEnumerable<IBackImageResolver> Create(IServiceProvider services)
            {
                var options = services.GetRequiredService<ParserConfig>();

                yield return new UrlFileBackImageResolver();

                if (!string.IsNullOrWhiteSpace(options.ImgurClientKey))
                {
                    yield return new ImgurBackImageResolver(options.ImgurClientKey);
                }

                yield return new BackUrlBackImageResolver(options.BackUrl);
            }
        }
    }
}