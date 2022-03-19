using System;
using System.Collections;
using System.IO;
using DFS;

namespace BFS;

public class BreadthFirstSearch
{
    private bool _fileSearched;
    private int _timeSpent;
    private NTree<string> _searchTree;
    private Queue<string> _searchQueue;
    private string[] _filePaths;
}
