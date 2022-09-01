using System;
using System.IO;

namespace FlowDMApi.Core.Extentions
{
    public static class DirectoryExtensions
    {
        public static void Copy(this DirectoryInfo sourceDirectory, string targetDirectory)
        {
            var diTarget = new DirectoryInfo(targetDirectory);
            CopyAll(sourceDirectory, diTarget);
        }

        public static void CopyAll(this DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists; if not, create it.
            if (!target.Exists)
            {
                target.Create();
            }

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                diSourceSubDir.CopyAll(nextTargetSubDir);
            }
        }

        public static void Empty(this DirectoryInfo source)
        {
            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles())
            {
                fi.Delete();
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                diSourceSubDir.Delete(true);
            }
        }
    }
    public static class FileExtensions
    {
    }
}