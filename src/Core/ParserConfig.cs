namespace Core;

public class ParserConfig
{
    public string? FilePath { get; init; }
    public string ResultPath { get; init; }
    public string ImportPath { get; init; }
    public string CompletedPath { get; init; }
    public string[] FilePaths { get; init; }
    public string BackUrl { get; init; }
    public bool CleanDeckNames { get; init; }
    public bool MoveParsedFiles { get; init; }
    public string? ImgurClientKey { get; init; }

    public bool IsSingleFile => FilePath is not null;

    public ParserConfig(ParserFileConfig config, string? filePath = null)
    {
        FilePath = filePath;
        ResultPath = ExpandPath(config.ResultPath);
        ImportPath = ExpandPath(config.ImportPath);
        CompletedPath = ExpandPath(config.CompletedPath);

        FilePaths = FilePath is not null
            ? new[] { FilePath }
            : GetFilePaths(ImportPath);

        BackUrl = config.BackUrl;
        CleanDeckNames = config.CleanDeckNames;
        MoveParsedFiles = config.MoveParsedFiles;
        ImgurClientKey = !string.IsNullOrWhiteSpace(config.ImgurClientKey)
            ? config.ImgurClientKey
            : null;

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