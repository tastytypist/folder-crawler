using System;

namespace DFS
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    public class DepthFirstSearch
    {
        public static NTree<string> searchFolder(DirectoryInfo source, string target,List<string> path,out bool found, bool occurence)
        {
            // Check if the target directory exists, if not, create it.

            // Copy each file into it's new directory.
            found = false;
            NTree<string> tree = new NTree<string>(source.Name,0) ;
        

            // Copy each subdirectory using recursion.
            string subdir = @"";
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {

                NTree<string> file = searchFolder(diSourceSubDir, target,path,out found,occurence);
                tree.children.AddLast(file);
                if (found)
                {
                    tree.colour = 1;
                    if (!occurence) return tree;
                }
            }
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(fi);
                
                if (fi.Name == target)
                {
                    path.Add(fi.FullName);
                    tree.AddChild(fi.Name, 1);
                    tree.colour = 1;
                    found = true;
                    if (!occurence) return tree;
                }
                else
                {
                    tree.AddChild(fi.Name, 0);
                }
            }
            return tree;
        }


        // Output will vary based on the contents of the source directory.
    }
    

    //elegate void TreeVisitor<T>(T nodeData);

    public class NTree<T>
    {
        public T data;
        public int colour;// pewarnaan 0 merah, 1 biru, 2 hitam;
        public LinkedList<NTree<T>> children;

        public NTree(T data,int colour)
        {
            this.data = data;
            this.colour = colour;
            children = new LinkedList<NTree<T>>();
        }

        public void AddChild(T data, int colour)
        {
            children.AddFirst(new NTree<T>(data,colour));
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
            Console.Write(node.colour);
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
            string sourceDirectory = @"C:\Users\MSI GAMING\Documents\GitHub\Tubes-Alstrukdat";
            string targetDirectory = @"buy.c";
            List<string> path = new List<string>();
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            //    FileInfo diTarget = new FileInfo(targetDirectory);
            bool found = false;
            bool occurence = true;
            NTree<string> cari = DepthFirstSearch.searchFolder(diSource, targetDirectory,path,out found,occurence);
            cari.Traverse(cari,1);
            Console.WriteLine(path[0]);
            Console.WriteLine(path[1]);
            Console.WriteLine(found);
            //    Console.WriteLine(cari);
            //    FileInfo diTarget = new FileInfo(cari);
            Console.ReadLine();
            // Create a tree.
            
        }
    }
}