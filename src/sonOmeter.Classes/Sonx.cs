using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zip;

namespace sonOmeter.Classes
{
    public class Sonx
    {
        public static void Create(string file, string path)
        {
            using (ZipFile zip = new ZipFile(file))
            {
                // Get all files of a given path.
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                int max = files.Length;

                for (int i = 0; i < max; i++)
                {
                    // remove the common path from all files
                    string filePath = Path.GetDirectoryName(files[i]).Remove(0, path.Length - 1);
                    zip.AddFile(files[i], filePath);
                }

                zip.Save();
            }
        }

        public static void Extract(string file, string path)
        {
            using (ZipFile zip = ZipFile.Read(file))
            {
                zip.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}
