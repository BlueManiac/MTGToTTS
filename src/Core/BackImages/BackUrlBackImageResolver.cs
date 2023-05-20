using System.Threading;
using System.Threading.Tasks;

namespace Core.BackImages;

public class BackUrlBackImageResolver : IBackImageResolver
{
    public string _url { get; }

    public BackUrlBackImageResolver(string url)
    {
        _url = url;
    }

    public Task<string> Resolve(string deckFilePath, CancellationToken cancellationToken)
    {
        return WebUtil.ExpandUrl(_url);
    }
}