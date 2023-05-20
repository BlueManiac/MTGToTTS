using System.Threading;
using System.Threading.Tasks;

namespace Core.BackImages;

public interface IBackImageResolver
{
    Task<string> Resolve(string deckFilePath, CancellationToken cancellationToken);
}