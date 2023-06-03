using Core.Util;
using Imgur.API.Authentication;
using Imgur.API.Endpoints;

namespace Core.BackImages.Resolvers;

public class ImgurBackImageResolver : IBackImageResolver
{
    private readonly ImageEndpoint _imageEndpoint;

    public ImgurBackImageResolver(string imgurClientKey)
    {
        var apiClient = new ApiClient(imgurClientKey);
        var httpClient = new HttpClient();

        _imageEndpoint = new ImageEndpoint(apiClient, httpClient);
    }

    public async Task<string?> Resolve(string deckFilePath, CancellationToken cancellationToken)
    {
        var imageFilePath = RelatedImageResolver.Find(deckFilePath);

        if (imageFilePath == null)
        {
            return null;
        }

        using var fileStream = File.OpenRead(imageFilePath);

        var imageUpload = await _imageEndpoint.UploadImageAsync(fileStream, cancellationToken: cancellationToken);
        var link = imageUpload.Link;

        var urlFilePath = Path.ChangeExtension(deckFilePath, ".url");

        // Cache imgur location for next invocation
        await File.WriteAllTextAsync(urlFilePath, link, cancellationToken);

        return link;
    }
}