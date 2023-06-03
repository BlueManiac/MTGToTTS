using Core.Util;

namespace Core.BackImages.Resolvers;

public class BackUrlBackImageResolver : IBackImageResolver
{
    private readonly string _url;

    public BackUrlBackImageResolver(string url)
    {
        _url = url;
    }

    public Task<string?> Resolve(string deckFilePath, CancellationToken cancellationToken)
    {
        return WebUtil.ExpandUrl(_url)!;
    }
}