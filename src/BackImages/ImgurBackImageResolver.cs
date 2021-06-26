using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using Imgur.API.Authentication;
using Imgur.API.Endpoints;
using Imgur.API.Models;
using System.IO;

namespace DeckParser.BackImages
{
    public class ImgurBackImageResolver : IBackImageResolver
    {
        private readonly ImageEndpoint _imageEndpoint;

        public ImgurBackImageResolver(string imgurClientKey)
        {
            var apiClient = new ApiClient(imgurClientKey);
            var httpClient = new HttpClient();
            
            _imageEndpoint = new ImageEndpoint(apiClient, httpClient);
        }

        public async Task<string> Resolve(string imageFilePath, CancellationToken cancellationToken)
        {
            using var fileStream = File.OpenRead(imageFilePath);

            var imageUpload = await _imageEndpoint.UploadImageAsync(fileStream);

            return imageUpload.Link;
        }
    }
}