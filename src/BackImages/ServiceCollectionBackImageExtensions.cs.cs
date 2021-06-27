using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DeckParser.BackImages
{
    public static class ServiceCollectionBackImageExtensions
    {
        public static IServiceCollection RegisterBackImageResolvers(this IServiceCollection collection) {
            return collection
                .AddSingleton<IEnumerable<IBackImageResolver>>(Create);

            static IEnumerable<IBackImageResolver> Create(IServiceProvider services) {
                var options = services.GetRequiredService<Options>();

                yield return new UrlFileBackImageResolver();

                if (!string.IsNullOrWhiteSpace(options.ImgurClientKey)) {
                    yield return new ImgurBackImageResolver(options.ImgurClientKey);
                }

                yield return new BackUrlBackImageResolver(options.BackUrl);
            }
        }
    }
}