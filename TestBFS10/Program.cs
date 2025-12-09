using System;
using System.Collections.Generic;
using System.Diagnostics;

using IntPair = System.ValueTuple<int, int>;

Main();


void Main() {
    Console.WriteLine("Start BFS Test");

    var intGraph = new IntGraph();
    var path = BFS.GetPath(intGraph, 90, 95);
    var pathString = string.Join(",", path.ToArray());
    Console.WriteLine("path is: " + pathString);
    Debug.Assert(pathString == "90,91,92,93,94,95");
    path = BFS.GetPath(intGraph, 85, 80);
    pathString = string.Join(",", path.ToArray());
    Console.WriteLine("path is: " + pathString);
    Debug.Assert(pathString == "85,84,83,82,81,80");

    var intPairGraph = new IntPairGraph();
    var path2 = BFS.GetPath(intPairGraph, (9, 5), (7, 6));
    pathString = string.Join(",", path2.ToArray());
    Console.WriteLine("path is: " + pathString);
    Debug.Assert(pathString == "(9, 5),(9, 6),(8, 6),(7, 6)");
    Debug.Assert(path2.Count ==4);


    // Here we should get an empty path because of maxiterations:
    int maxiterations = 1000;
    path2 = BFS.GetPath(intPairGraph, (9, 5), (-7,-6), maxiterations); 
    pathString = string.Join(",", path2.ToArray());
    Console.WriteLine("path is: " + pathString);
    Debug.Assert(pathString == "");

    Console.WriteLine("End BFS Test");

    MockGraph graph = new MockGraph();
    Console.WriteLine("--- Running BFS.CountReachableNodes Unit Tests ---");

    // 1. בדיקה: אזור מחובר גדול (צפוי 3 צמתים נגישים)
    int count1 = BFS.CountReachableNodes(graph, 1);
    
    if (count1 == 3)
        Console.WriteLine("Test 1 (Connected Area) PASS: Count = 3");
    else
        Console.WriteLine($"Test 1 (Connected Area) FAIL: Expected 3, Got {count1}");


    // 2. בדיקה: אזור מבודד קטן (צפוי 2 צמתים נגישים)
    int count2 = BFS.CountReachableNodes(graph, 4);
    
    if (count2 == 2)
        Console.WriteLine("Test 2 (Disconnected Area) PASS: Count = 2");
    else
        Console.WriteLine($"Test 2 (Disconnected Area) FAIL: Expected 2, Got {count2}");

    int count3 = BFS.CountReachableNodes(graph, 99);
    Console.WriteLine($"Test 3 (Start 99): Reachable Count = {count3}");
    
    // בדיקת יחידה: מוודאים ש-1 צמתים נגישים (צומת ההתחלה עצמו)
    if (count3 == 1)
        Console.WriteLine("Test 3 (Non-Existent Node) PASS");
    else
        Console.WriteLine($"Test 3 (Non-Existent Node) FAIL: Expected 1, Got {count3}"); // <--- תיקון ה-Expected
}

class IntGraph: IGraph<int>
{  // IGraph is defined in the file ./IGraph.cs (a copy is found in Assets/Scripts/0-bfs/IGraph.cs)
    public IEnumerable<int> Neighbors(int node) {
        yield return node + 1;
        yield return node - 1;
    }
}

class IntPairGraph : IGraph<IntPair> {
    public IEnumerable<IntPair> Neighbors(IntPair node) {
        yield return (node.Item1, node.Item2 + 1);
        yield return (node.Item1, node.Item2 - 1);
        yield return (node.Item1 + 1, node.Item2);
        yield return (node.Item1 - 1, node.Item2);
        //yield return (node.Item1 - 1, node.Item2+1);
    }
}
public class MockGraph : IGraph<int>
{
    private Dictionary<int, List<int>> adjacencyList;

    public MockGraph()
    {
        // גרף עם שני אזורים מבודדים: (1-2-3) ו- (4-5)
        adjacencyList = new Dictionary<int, List<int>>
        {
            {1, new List<int> {2, 3}}, 
            {2, new List<int> {1}},    
            {3, new List<int> {1}},    
            {4, new List<int> {5}},    
            {5, new List<int> {4}}
        };
    }

    public IEnumerable<int> Neighbors(int node)
    {
        if (adjacencyList.ContainsKey(node))
            return adjacencyList[node];
        return Enumerable.Empty<int>();
    }
}
