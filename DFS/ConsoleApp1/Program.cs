using System;

namespace RectangleApplication
{
    using System;
    using System.IO;

    class CopyDir
    {
        public static string searchFolder(DirectoryInfo source, string target)
        {
            // Check if the target directory exists, if not, create it.

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(fi);
                if (fi.Name == target)
                {
                    return fi.FullName;
                }
            }

            // Copy each subdirectory using recursion.
            string subdir = @"";
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    source.CreateSubdirectory(diSourceSubDir.Name);
                string file = searchFolder(diSourceSubDir, target); 
                if (file!= "")
                {
                    return file;
                } 
            }
            return subdir;
        }

        public static void Main()
        {
            string sourceDirectory = @"D:\Game";
            string targetDirectory = @"Battlefield 1.rar";

            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
        //    FileInfo diTarget = new FileInfo(targetDirectory);
            
            string cari = searchFolder(diSource,targetDirectory);
            Console.WriteLine(cari);
            FileInfo diTarget = new FileInfo(cari);
            Console.ReadLine();
        }

        // Output will vary based on the contents of the source directory.
    }
}