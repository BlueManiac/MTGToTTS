using System.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class RelatedImageResolver {
    private static readonly string[] _imageExtensions = {"jpg", "bmp", "gif", "png", "jpeg"};

    public static string Find(string filePath) {
        foreach (var ext in _imageExtensions)
        {
            var imageFilePath = Path.ChangeExtension(filePath, ext);

            if (File.Exists(imageFilePath)) {
                return imageFilePath;
            }
        }

        return null;
    }
}