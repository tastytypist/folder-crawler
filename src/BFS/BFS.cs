using System.Diagnostics;
using DFS;

namespace BFS;

public class BreadthFirstSearch
{
    private bool _fileFound;
    private readonly bool _findMultipleOccurence;
    private int _fileFoundDepth;
    private readonly List<string> _filePaths;
    private NTree<FileSystemInfo> _searchTree;
    private readonly Queue<NTree<FileSystemInfo>> _searchQueue;
    private readonly Stopwatch _timeSpent;

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

    public Tuple<List<string>, NTree<FileSystemInfo>, long> BreadthSearchFile
        (DirectoryInfo startDirectory, string targetFile)
    {
        _searchTree = BuildTree(startDirectory);
        
        _timeSpent.Start();
        SearchFile(targetFile);
        _timeSpent.Stop();
        /*
        if (!_findMultipleOccurence)
        {
            var currentLevel = 0;
            _searchTree = NTree<FileSystemInfo>.PurgeChild(_searchTree, _fileFoundDepth, ref currentLevel);
        }*/

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

    private void SearchFile(string targetFile)
    {
        _searchQueue.Enqueue(_searchTree);

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
                    _fileFoundDepth++;
                    foreach (var entry in currentEntry.children)
                    {
                        _searchQueue.Enqueue(entry);
                    }
                    break;
            }
        }
    }

    private NTree<FileSystemInfo> ColourTree
        (DirectoryInfo startDirectory, ref bool isColoured, ref int currentDepth)
    {
        var searchTree = new NTree<FileSystemInfo>(startDirectory, 0);

        if (currentDepth >= _fileFoundDepth)
        {
            currentDepth--;
            return searchTree;
        }
        currentDepth++;
        foreach (var file in startDirectory.GetFiles())
        {
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
        }

        foreach (var directory in startDirectory.GetDirectories())
        {
            var subdirectory = BuildTree(directory);
            if (isColoured && currentDepth == _fileFoundDepth)
            {
                subdirectory.colour = 2;
            }
                
            searchTree.children.AddLast(subdirectory);
            if (subdirectory.colour == 1)
            {
                searchTree.colour = 1;
            }
        }

        currentDepth--;
        return searchTree;
    }
    
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
            var subdirectory = BuildTree(directory);
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
