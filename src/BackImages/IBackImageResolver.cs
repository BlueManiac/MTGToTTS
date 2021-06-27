using System.Threading;
using System.Threading.Tasks;

namespace DeckParser.BackImages
{
    public interface IBackImageResolver
    {
        Task<string> Resolve(string deckFilePath, CancellationToken cancellationToken);
    }
}