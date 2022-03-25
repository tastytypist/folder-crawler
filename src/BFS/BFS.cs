using System.Diagnostics;
using DFS;

namespace BFS;

/// <summary>
/// Class <c>BreadthFirstSearch</c> represents the object that facilitates
/// file Breadth First Search.
/// </summary>
public class BreadthFirstSearch
{
    private bool _fileFound;
    private readonly bool _findMultipleOccurence;
    private int _fileFoundDepth;
    private readonly List<string> _filePaths;
    private NTree<FileSystemInfo> _searchTree;
    private readonly Queue<NTree<FileSystemInfo>> _searchQueue;
    private readonly Stopwatch _timeSpent;

    /// <summary>
    /// Constructor for class <c>BreadthFirstSearch</c>.
    /// </summary>
    /// <param name="findMultipleOccurence"><c>true</c> if searching for all
    /// occurrences within a directory tree, <c>false</c> otherwise.</param>
    public BreadthFirstSearch(bool findMultipleOccurence = false)
    {
        _fileFound = false;
        _findMultipleOccurence = findMultipleOccurence;
        _fileFoundDepth = 0;
        _filePaths = new List<string>();
        _searchTree = new NTree<FileSystemInfo>(new DirectoryInfo(Directory.GetCurrentDirectory()), 0);
        _searchQueue = new Queue<NTree<FileSystemInfo>>();
        _timeSpent = new Stopwatch();
    }

    /// <summary>
    /// Search a given file using breadth-first search.
    /// </summary>
    /// <param name="startDirectory">root node for the directory tree.</param>
    /// <param name="targetFile">name of the target file.</param>
    /// <returns>A <c>Tuple</c> consisting of list of found file paths,
    /// coloured directory tree, and time elapsed during searching.</returns>
    public Tuple<List<string>, NTree<FileSystemInfo>, long> BreadthSearchFile
        (DirectoryInfo startDirectory, string targetFile)
    {
        _searchTree = BuildTree(startDirectory);
        
        _timeSpent.Start();
        SearchFile(targetFile);
        _timeSpent.Stop();

        if (!_fileFound)
            return new Tuple<List<string>, NTree<FileSystemInfo>, long>(_filePaths, _searchTree,
                _timeSpent.ElapsedMilliseconds);
        switch (_findMultipleOccurence)
        {
            case false:
                var colourDepth = 0;
                var fileIsColoured = false;
                _searchTree = ColourTree((DirectoryInfo)_searchTree.data, ref fileIsColoured, ref colourDepth);
                break;
            case true:
                _searchTree = ColourMultiTree((DirectoryInfo)_searchTree.data);
                break;
        }

        return new Tuple<List<string>, NTree<FileSystemInfo>, long>(_filePaths, _searchTree, _timeSpent.ElapsedMilliseconds);
    }

    /// <summary>
    /// Build a full directory tree to be searched in.
    /// </summary>
    /// <param name="startDirectory">root node of the directory tree.</param>
    /// <returns>A full directory tree with <paramref name="startDirectory"/>
    /// as its root.</returns>
    private static NTree<FileSystemInfo> BuildTree(DirectoryInfo startDirectory)
    {
        var searchTree = new NTree<FileSystemInfo>(startDirectory, 0);

        foreach (var file in startDirectory.GetFiles())
        {
            searchTree.children.AddLast(new NTree<FileSystemInfo>(file, 0));
        }

        foreach (var directory in startDirectory.GetDirectories())
        {
            var subdirectory = BuildTree(directory);
            searchTree.children.AddLast(subdirectory);
        }

        return searchTree;
    }

    /// <summary>
    /// Look for the target file in the directory tree.
    /// </summary>
    /// <param name="targetFile">the name of the file being looked for.</param>
    private void SearchFile(string targetFile)
    {
        _searchQueue.Enqueue(_searchTree);
        var levelCountdown = 1;
        var directoryCounter = 0;

        while (_searchQueue.Count != 0 && (!_fileFound || _findMultipleOccurence))
        {
            var currentEntry = _searchQueue.Dequeue();
            switch (currentEntry.data)
            {
                case FileInfo file when file.Name == targetFile:
                    _fileFound = true;
                    _filePaths.Add(file.FullName);
                    break;
                case DirectoryInfo:
                    foreach (var entry in currentEntry.children)
                    {
                        _searchQueue.Enqueue(entry);
                        if (entry.data is DirectoryInfo)
                        {
                            directoryCounter++;
                        }
                    }
                    
                    levelCountdown--;
                    if (levelCountdown == 0)
                    {
                        _fileFoundDepth++;
                        levelCountdown += directoryCounter;
                        directoryCounter = 0;
                    }
                    
                    break;
            }
        }
    }

    /// <summary>
    /// Create a coloured directory tree for first-occurrence searches.
    /// </summary>
    /// <param name="startDirectory">root of the directory tree.</param>
    /// <param name="isColoured"><c>true</c> if the file being targeted has
    /// been found, <c>false</c> otherwise.</param>
    /// <param name="currentDepth">current depth of tree being coloured.</param>
    /// <returns>A coloured directory tree that indicates entries where the
    /// file isn't found, entries where the file is found, and entries in the
    /// queue ready to be checked.</returns>
    private NTree<FileSystemInfo> ColourTree
        (DirectoryInfo startDirectory, ref bool isColoured, ref int currentDepth)
    {
        var searchTree = new NTree<FileSystemInfo>(startDirectory, 0);

        if (currentDepth >= _fileFoundDepth)
        {
            return searchTree;
        }

        foreach (var file in startDirectory.GetFiles())
        {
            currentDepth++;
            if (isColoured && currentDepth == _fileFoundDepth)
            {
                searchTree.children.AddLast(new NTree<FileSystemInfo>(file, 2));
            }
            else if (_filePaths.Contains(file.FullName))
            {
                isColoured = true;
                searchTree.children.AddLast(new NTree<FileSystemInfo>(file, 1));
                searchTree.colour = 1;
            }
            else
            {
                searchTree.children.AddLast(new NTree<FileSystemInfo>(file, 0));
            }
            currentDepth--;
        }

        foreach (var directory in startDirectory.GetDirectories())
        {
            currentDepth++;
            var subdirectory = ColourTree(directory, ref isColoured, ref currentDepth);
            if (isColoured && currentDepth == _fileFoundDepth)
            {
                subdirectory.colour = 2;
            }
                
            searchTree.children.AddLast(subdirectory);
            if (subdirectory.colour == 1)
            {
                searchTree.colour = 1;
            }
            currentDepth--;
        }
        
        return searchTree;
    }
    
    /// <summary>
    /// Create a coloured directory tree for all-occurrence searches.
    /// </summary>
    /// <param name="startDirectory">root of the directory tree.</param>
    /// <returns>A coloured directory tree that indicates entries where the
    /// file isn't found and entries where the file is found.</returns>
    private NTree<FileSystemInfo> ColourMultiTree(DirectoryInfo startDirectory)
    {
        var searchTree = new NTree<FileSystemInfo>(startDirectory, 0);

        foreach (var file in startDirectory.GetFiles())
        {
            if (_filePaths.Contains(file.FullName))
            {
                searchTree.children.AddLast(new NTree<FileSystemInfo>(file, 1));
                searchTree.colour = 1;
            }
            else
            {
                searchTree.children.AddLast(new NTree<FileSystemInfo>(file, 0));
            }
        }

        foreach (var directory in startDirectory.GetDirectories())
        {
            var subdirectory = ColourMultiTree(directory);
            searchTree.children.AddLast(subdirectory);
            if (subdirectory.colour == 1)
            {
                searchTree.colour = 1;
            }
        }

        return searchTree;
    }
}

public static class BreadthTest
{
    public static void Main()
    {
        //const string start = @"C:\Users\Nathan\Documents";
        //const string goal = @"htw.wav";
        const string start = @"D:\Tugas Kuliah\Test";
        const string goal = @"M.txt";
        var startSource = new DirectoryInfo(start);

        var searcher = new BreadthFirstSearch();
        var (paths, tree, time) = searcher.BreadthSearchFile(startSource, goal);
        tree.Traverse(tree, 1);
        Console.WriteLine(paths);
        Console.WriteLine(time);
    }
}
