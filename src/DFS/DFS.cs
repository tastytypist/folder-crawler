using System;

namespace DFS
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    public class found
    {
        public bool find;
        public found(bool find)
        {
            this.find = find;
        }
    }
    public class DepthFirstSearch
    {
        public static NTree<FileSystemInfo> searchFolder(DirectoryInfo source, string target,List<string> path,found found, bool occurence)
        {
            // Check if the target directory exists, if not, create it.

            // Copy each file into it's new directory.
            
            NTree<FileSystemInfo> tree = new NTree<FileSystemInfo>(source,2) ;
            if (!occurence && found.find) return tree;

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {

                NTree<FileSystemInfo> file = searchFolder(diSourceSubDir, target, path, found, occurence);
                tree.children.AddLast(file);
                if (occurence)
                {
                    if (file.colour == 1)
                    {
                        tree.colour = 1;
                    }
                    else if(tree.colour == 2 && file.colour == 0)
                    {
                        tree.colour = 0;
                    }
                }
                else if (!occurence)
                {
                    if (found.find && file.colour ==1)
                    {
                        tree.colour = 1;
                    }
                    else if (found.find && file.colour == 0 && tree.colour != 1)
                    {
                        tree.colour = 0;
                    }
                }
            }
            foreach (FileInfo fi in source.GetFiles())
            {
                
                if (occurence)
                {
                    if (fi.Name == target)
                    {
                        path.Add(fi.FullName);
                        tree.AddChild(fi, 1);
                        tree.colour = 1;
                        found.find = true;
                    }
                    else
                    {
                        tree.AddChild(fi, 0);
                        if (tree.colour != 1) tree.colour = 0;
                    }
                }
                else if (!occurence)
                {
                    if (fi.Name == target && !found.find)
                    {
                        path.Add(fi.FullName);
                        tree.AddChild(fi, 1);
                        tree.colour = 1;
                        found.find = true;
                        return tree;
                    }
                    else if (found.find)
                    {
                        tree.AddChild(fi, 2);
                        return tree;

                    }

                    else if(!found.find)
                    {
                        tree.AddChild(fi, 0);
                    }
                }
            }
            if (tree.colour == 2 && occurence) tree.colour = 0;
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

        public static NTree<T> PurgeChild(NTree<T> node, int targetLevel, ref int currentLevel)
        {
            if (currentLevel == targetLevel)
            {
                node.children = new LinkedList<NTree<T>>();
                currentLevel--;
                return node;
            }
            currentLevel++;
            foreach (var entry in node.children)
            {
                PurgeChild(entry, targetLevel, ref currentLevel);
            }

            return node;
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
            found found = new found(false);
            bool occurence = true;
            NTree<FileSystemInfo> cari = DepthFirstSearch.searchFolder(diSource, targetDirectory,path,found,occurence);
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