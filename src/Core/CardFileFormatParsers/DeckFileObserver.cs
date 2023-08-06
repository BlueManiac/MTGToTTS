using Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CardFileFormatParsers;
public class DeckFileObserver
{
    private readonly ParserConfig _config;

    public DeckFileObserver(ParserConfig config)
    {
        _config = config;
    }

    public bool MoveParsedFile(string filePath)
    {
        if (!_config.MoveParsedFiles)
            return false;

        Directory.CreateDirectory(_config.CompletedPath);

        var destFilePath = Path.Combine(_config.CompletedPath, Path.GetFileName(filePath));

        File.Move(filePath, destFilePath, true);

        return true;
    }

    public IAsyncEnumerable<(string FilePath, WatcherChangeTypes ChangeType)> ObserveImportDirectory(CancellationToken cancellationToken)
    {
        var fileWatcher = new FileSystemWatcher(_config.ImportPath, "*.csv")
        {
            IncludeSubdirectories = false,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };

        return fileWatcher
            .ObserveFileChangesAsync(cancellationToken)
            .Where(x => x.ChangeType == WatcherChangeTypes.Created);
    }
}
