using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Core;

public class ParserConfig
{
    public string? FilePath { get; init; }
    public string ResultPath { get; init; }
    public string ImportPath { get; init; }
    public string[] FilePaths { get; init; }
    public string BackUrl { get; init; }
    public string? ImgurClientKey { get; init; }

    public bool IsSingleFile => FilePath is not null;

    public ParserConfig(ParserFileConfig config, string? filePath = null)
    {
        FilePath = filePath;
        ImportPath = ExpandPath(config.ImportPath);
        ResultPath = ExpandPath(config.ResultPath);

        FilePaths = FilePath is not null
            ? new[] { FilePath }
            : GetFilePaths(ImportPath);

        BackUrl = config.BackUrl;
        ImgurClientKey = config.ImgurClientKey;

        static string ExpandPath(string path)
        {
            return Path.GetFullPath(Environment.ExpandEnvironmentVariables(path ?? ""));
        }

        static string[] GetFilePaths(string path)
        {
            Directory.CreateDirectory(path);

            return Directory
                .GetFiles(path)
                .Select(x => Path.Combine(path, x))
                .ToArray();
        }
    }
}