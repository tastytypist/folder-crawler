using System;

namespace DFS
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    class CopyDir
    {
        public static NTree<string> searchFolder(DirectoryInfo source, string target,List<string> path)
        {
            // Check if the target directory exists, if not, create it.

            // Copy each file into it's new directory.

            NTree<string> tree = new NTree<string>(source.FullName) ;
        

            // Copy each subdirectory using recursion.
            string subdir = @"";
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {

                NTree<string> file = searchFolder(diSourceSubDir, target,path);
                tree.children.AddLast(file); 
             //   if (file!= "")
             //    {
             //       return file;
             //   } 
            }
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(fi);
                tree.AddChild(fi.FullName);
                if (fi.Name == target)
                {
                    path.Add(fi.FullName);
                }
            }
            return tree;
        }


        // Output will vary based on the contents of the source directory.
    }
    

    //elegate void TreeVisitor<T>(T nodeData);

    class NTree<T>
    {
        public T data;
        public LinkedList<NTree<T>> children;

        public NTree(T data)
        {
            this.data = data;
            children = new LinkedList<NTree<T>>();
        }

        public void AddChild(T data)
        {
            children.AddFirst(new NTree<T>(data));
        }

        public NTree<T> GetChild(int i)
        {
            foreach (NTree<T> n in children)
                if (--i == 0)
                    return n;
            return null;
        }

        public void Traverse(NTree<T> node,int i)
        {
            Console.Write(i);
            Console.Write(". ");
            Console.Write(node.data);
            Console.WriteLine();
            foreach (NTree<T> kid in node.children)
                Traverse(kid,i+1);
        }
    }
    class Program
    {
        static void Main()
        {
            string sourceDirectory = @"D:\Game";
            string targetDirectory = @"Battlefield 1.rar";
            List<string> path = new List<string>();
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            //    FileInfo diTarget = new FileInfo(targetDirectory);

            NTree<string> cari = CopyDir.searchFolder(diSource, targetDirectory,path);
            cari.Traverse(cari,1);
            Console.WriteLine(path[0]);
            Console.WriteLine(path[1]);
            //    Console.WriteLine(cari);
            //    FileInfo diTarget = new FileInfo(cari);
            Console.ReadLine();
            // Create a tree.
            
        }
    }
}