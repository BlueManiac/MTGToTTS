using System;
using System.Runtime.InteropServices.ComTypes;
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

                if (!string.IsNullOrWhiteSpace(options.ImgurClientKey)) {
                    yield return new ImgurBackImageResolver(options.ImgurClientKey);
                }
            }
        }
    }
}