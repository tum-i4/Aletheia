using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectralizer.HIL.Tools
{
    public class FileExtractor
    {
        private string directoryRootPath;

        private Dictionary<string, string> filePathsWithFilenameAsKey;

        public FileExtractor(string directoryRootPath)
        {
            this.directoryRootPath = directoryRootPath;
            filePathsWithFilenameAsKey = new Dictionary<string, string>();

            searchAllFilesInDirectory(directoryRootPath);
        }

        private void searchAllFilesInDirectory(string directoryPath)
        {
            foreach (string path in Directory.GetFiles(directoryPath))
            {
                FileAttributes attr = File.GetAttributes(path);
                if (File.Exists(path) && !attr.HasFlag(FileAttributes.Directory))
                {
                    FileInfo file = new FileInfo(path);
                    string extension = file.Extension;
                    if (extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
                    {
                        string filename = Path.GetFileNameWithoutExtension(path);
                        filePathsWithFilenameAsKey.Add(filename, path);
                    }
                }
            }

            foreach (string path in Directory.GetDirectories(directoryPath))
            {
                searchAllFilesInDirectory(path);
            }
        }

        public Dictionary<string, string> AllFilePathsWithFileNameAsKey
        {
            get { return filePathsWithFilenameAsKey; }
        }
    }
}
