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
    private readonly Queue<NTree<object>> _searchQueue;
    private readonly Stopwatch _timeSpent;

    public BreadthFirstSearch(bool findMultipleOccurence = false)
    {
        _fileFound = false;
        _findMultipleOccurence = findMultipleOccurence;
        _fileFoundDepth = 0;
        _filePaths = new List<string>();
        _searchTree = new NTree<object>(@"", 0, @"");
        _searchQueue = new Queue<NTree<object>>();
        _timeSpent = new Stopwatch();
    }

    public Tuple<List<string>, NTree<object>, long> BreadthSearchFile
        (DirectoryInfo startDirectory, string targetFile)
    {
        _searchTree = BuildTree(startDirectory);
        
        _timeSpent.Start();
        SearchFile(targetFile);
        _timeSpent.Stop();

        return new Tuple<List<string>, NTree<object>, long>(_filePaths, _searchTree, _timeSpent.ElapsedMilliseconds);
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
