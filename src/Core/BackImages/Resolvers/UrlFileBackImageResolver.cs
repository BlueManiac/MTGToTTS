namespace Core.BackImages.Resolvers;

public class UrlFileBackImageResolver : IBackImageResolver
{
    public async Task<string?> Resolve(string deckFilePath, CancellationToken cancellationToken)
    {
        var urlFilePath = Path.ChangeExtension(deckFilePath, ".url");

        if (!File.Exists(urlFilePath))
        {
            return null;
        }

        return await File.ReadAllTextAsync(urlFilePath, cancellationToken);
    }
}