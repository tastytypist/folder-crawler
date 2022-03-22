using System.Diagnostics;
using DFS;

namespace BFS;

public class BreadthFirstSearch
{
    private bool _fileFound;
    private readonly bool _findMultipleOccurence;
    private int _fileFoundDepth;
    private readonly List<string> _filePaths;
    private NTree<object> _searchTree;
    private readonly Queue<FileSystemInfo> _searchQueue;
    private readonly Stopwatch _timeSpent;

    public BreadthFirstSearch(bool findMultipleOccurence = false)
    {
        _fileFound = false;
        _findMultipleOccurence = findMultipleOccurence;
        _filePaths = new List<string>();
        _searchTree = new NTree<object>(@"", 0, @"");
        _searchQueue = new Queue<FileSystemInfo>();
        _timeSpent = new Stopwatch();
    }

    public Tuple<List<string>, NTree<object>, TimeSpan> BreadthSearchFile
        (DirectoryInfo startDirectory, string targetFile)
    {
        _searchTree = BuildTree(startDirectory);
        
        _timeSpent.Start();
        _searchTree = SearchFile(startDirectory, targetFile);
        _timeSpent.Stop();

        return new Tuple<List<string>, NTree<object>, TimeSpan>(_filePaths, _searchTree, _timeSpent.Elapsed);
    }

    private static NTree<object> BuildTree(DirectoryInfo startDirectory)
    {
        var searchTree = new NTree<object>(startDirectory, 0, startDirectory.FullName);

        foreach (var file in startDirectory.GetFiles())
        {
            searchTree.children.AddLast(new NTree<object>(file, 0, file.FullName));
        }

        foreach (var directory in startDirectory.GetDirectories())
        {
            var subdirectory = BuildTree(directory);
            searchTree.children.AddLast(subdirectory);
        }

        return searchTree;
    }

    private NTree<object> SearchFile(DirectoryInfo startDirectory, string targetFile)
    {
        var searchTree = new NTree<object>(startDirectory, 0, startDirectory.FullName);
        
        foreach (var file in startDirectory.GetFiles())
        {
            _searchQueue.Enqueue(file);
        }
        
        foreach (var directory in startDirectory.GetDirectories())
        {
            _searchQueue.Enqueue(directory);
        }

        foreach (var entry in searchTree.children)
        {
            if (_fileFound && !_findMultipleOccurence)
            {
                return searchTree;
            }
            switch (entry.data)
            {
                case FileInfo file:
                    if (file.Name == targetFile)
                    {
                        _fileFound = true;
                        _filePaths.Add(file.FullName);
                        entry.colour = 1;

                        if (!_findMultipleOccurence)
                        {
                            return searchTree;
                        }
                    }
                    break;
                case DirectoryInfo directory:
                    _searchQueue.Enqueue(directory);
                    break;
            }
        }

        return searchTree;
    }
}

public class BreadthTest
{
    public static void Main()
    {
        const string start = @"C:\Users\Nathan\Documents";
        const string goal = @"htw.wav";
        var startSource = new DirectoryInfo(start);

        var searcher = new BreadthFirstSearch();
        var (paths, tree, time) = searcher.BreadthSearchFile(startSource, goal);
        Console.WriteLine(paths);
        Console.WriteLine(tree);
        Console.WriteLine(time);
    }
}
