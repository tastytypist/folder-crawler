using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using DFS;

namespace BFS;

public class BreadthFirstSearch
{
    private bool _fileFound;
    private readonly bool _findMultipleOccurence;
    private readonly List<string> _filePaths;
    private NTree<object> _searchTree;
    private readonly Queue<FileInfo> _searchQueue;
    private readonly Stopwatch _timeSpent;

    public BreadthFirstSearch(bool findMultipleOccurence = false)
    {
        _fileFound = false;
        _findMultipleOccurence = findMultipleOccurence;
        _filePaths = new List<string>();
        _searchTree = new NTree<object>(@"", 0, @"");
        _searchQueue = new Queue<FileInfo>();
        _timeSpent = new Stopwatch();
    }

    public Tuple<List<string>, NTree<object>, TimeSpan> BreadthSearchFile
        (DirectoryInfo startDirectory, string targetFile)
    {
        _searchTree = new NTree<object>(startDirectory, 0, startDirectory.FullName);
        
        _timeSpent.Start();
        SearchFile(startDirectory, targetFile);
        _timeSpent.Stop();

        return new Tuple<List<string>, NTree<object>, TimeSpan>(_filePaths, _searchTree, _timeSpent.Elapsed);
    }

    private void SearchFile(DirectoryInfo startDirectory, string targetFile)
    {
        if (_fileFound && !_findMultipleOccurence)
        {
            return;
        }
        else
        {
            foreach (var directory in startDirectory.GetDirectories())
            {
                _searchTree.AddChild(directory, 0, directory.FullName);
            }
        
            foreach (var file in startDirectory.GetFiles())
            {
                _searchQueue.Enqueue(file);
                _searchTree.AddChild(file, 0, file.FullName);
            }

            foreach (var entry in _searchTree.children)
            {
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
                                return;
                            }
                        }
                        break;
                    case DirectoryInfo directory:
                        SearchFile(directory, targetFile);
                        break;
                }
            }
        }
    }
}
