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
        public string ImportPath { get; set; } = "Decks";
        internal string FilePath { get; set; }
        internal string[] FilePaths {  get; private set; }
        internal bool IsSingleFile { get; private set; }

        public async Task Expand() {
            BackUrl = await WebUtil.ExpandUrl(BackUrl);
            ImportPath = ExpandPath(ImportPath);
            ResultPath = ExpandPath(ResultPath);
            
            IsSingleFile = FilePath != null;
            FilePaths = IsSingleFile
                ? new [] { FilePath }
                : GetFilePaths(ImportPath);
            
            static string ExpandPath(string path) {
                return Path.GetFullPath(Environment.ExpandEnvironmentVariables(path ?? ""));
            }

            static string[] GetFilePaths(string path) {
                Directory.CreateDirectory(path);

                return Directory
                    .GetFiles(path)
                    .Select(x => Path.Combine(path, x))
                    .ToArray();
            }
        }
    }
}