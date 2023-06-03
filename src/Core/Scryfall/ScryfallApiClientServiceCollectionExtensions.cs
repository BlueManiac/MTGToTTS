using Microsoft.Extensions.DependencyInjection;

namespace Core.Scryfall;
public static class ScryfallApiClientServiceCollectionExtensions
{
    public static void AddScryfallApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<ScryfallApiClient>(client => client.BaseAddress = new Uri("https://api.scryfall.com/"));
    }
}
