using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using Imgur.API.Authentication;
using Imgur.API.Endpoints;
using System.IO;

namespace Core.BackImages;

public class ImgurBackImageResolver : IBackImageResolver
{
    private readonly ImageEndpoint _imageEndpoint;

    public ImgurBackImageResolver(string imgurClientKey)
    {
        var apiClient = new ApiClient(imgurClientKey);
        var httpClient = new HttpClient();

        _imageEndpoint = new ImageEndpoint(apiClient, httpClient);
    }

    public async Task<string> Resolve(string deckFilePath, CancellationToken cancellationToken)
    {
        var imageFilePath = RelatedImageResolver.Find(deckFilePath);

        if (imageFilePath == null)
        {
            return null;
        }

        using var fileStream = File.OpenRead(imageFilePath);

        var imageUpload = await _imageEndpoint.UploadImageAsync(fileStream);
        var link = imageUpload.Link;

        var urlFilePath = Path.ChangeExtension(deckFilePath, ".url");

        // Cache imgur location for next invocation
        await File.WriteAllTextAsync(urlFilePath, link);

        return link;
    }
}