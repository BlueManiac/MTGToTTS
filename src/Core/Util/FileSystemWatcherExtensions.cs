using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Core.Util;
public static class FileSystemWatcherExtensions
{
    public static async IAsyncEnumerable<(string FilePath, WatcherChangeTypes ChangeType)> ObserveFileChangesAsync(this FileSystemWatcher fileWatcher, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var eventQueue = new ConcurrentQueue<(string, WatcherChangeTypes)>();
        var queueSemaphore = new SemaphoreSlim(0);

        fileWatcher.Created += (_, e) => EnqueueFileEvent(e);
        fileWatcher.Changed += (_, e) => EnqueueFileEvent(e);
        fileWatcher.Deleted += (_, e) => EnqueueFileEvent(e);

        fileWatcher.EnableRaisingEvents = true;

        while (!cancellationToken.IsCancellationRequested)
        {
            await queueSemaphore.WaitAsync(cancellationToken);

            if (eventQueue.TryDequeue(out var fileEvent))
            {
                yield return fileEvent;
            }
        }

        fileWatcher.Dispose();

        void EnqueueFileEvent(FileSystemEventArgs e)
        {
            eventQueue.Enqueue((e.FullPath, e.ChangeType));
            queueSemaphore.Release();
        }
    }
}
