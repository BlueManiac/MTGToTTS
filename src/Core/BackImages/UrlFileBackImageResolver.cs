using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Core.BackImages;

public class UrlFileBackImageResolver : IBackImageResolver
{
    public async Task<string> Resolve(string deckFilePath, CancellationToken cancellationToken)
    {
        var urlFilePath = Path.ChangeExtension(deckFilePath, ".url");

        if (!File.Exists(urlFilePath))
        {
            return null;
        }

        return await File.ReadAllTextAsync(urlFilePath);
    }
}