using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeckParser
{
    public class Options
    {
        public string BackUrl { get; set; } = "https://loremflickr.com/480/680";
        public string ResultPath { get; set; } = @"%USERPROFILE%\Documents\My Games\Tabletop Simulator\Saves\Saved Objects\Imported";
        public string FilePath { get; set; }
        public IEnumerable<string> FilePaths { get; private set; }
        public string DelverLensFolder { get; set; } = "DelverLensDecks";

        public async Task Expand() {
            BackUrl = await WebUtil.ExpandUrl(BackUrl);
            ResultPath = Environment.ExpandEnvironmentVariables(ResultPath);
            FilePaths = FilePath != null
                ? Enumerable.Empty<string>().Append(FilePath)
                : GetFilePaths(DelverLensFolder);
            
            static IEnumerable<string> GetFilePaths(string folder, string path = null) {
                path = path ?? Directory.GetCurrentDirectory();

                Directory.CreateDirectory(Path.Combine(path, folder));

                return Directory
                    .GetFiles(folder)
                    .Select(x => Path.Combine(path, x));
            }
        }
    }
}