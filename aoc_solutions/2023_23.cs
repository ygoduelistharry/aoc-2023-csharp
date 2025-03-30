class AoC2023_23 : AoCSolution
{
    record struct Point(int R, int C);

    static Dictionary<Point, Dictionary<Point, int>> ReduceToGraph(
        string[] input,
        Point startPos,
        Point endPos,
        bool slopes = true)
    {
        Dictionary<Point, Dictionary<Point, int>> graphNodes = [];
        graphNodes.Add(startPos, []);
        graphNodes.Add(endPos, []);

        Queue<(Point currPos, Point lastPos, Point lastNode, int dist)> queue = [];
        queue.Enqueue((new Point(1, 1), startPos, startPos, 1));

        while (queue.Count > 0)
        {
            var (currPos, lastPos, lastNode, dist) = queue.Dequeue();

            if (graphNodes.ContainsKey(currPos))
            {
                if (!graphNodes[lastNode].TryAdd(currPos, dist))
                {
                    graphNodes[lastNode][currPos] = dist;
                }

                if (!slopes)
                {
                    if (!graphNodes[currPos].TryAdd(lastNode, dist))
                    {
                        graphNodes[currPos][lastNode] = dist;
                    }
                }

                continue;
            }

            List<Point> nextPoints = [
                new Point(currPos.R + 1, currPos.C),
                new Point(currPos.R - 1, currPos.C),
                new Point(currPos.R, currPos.C + 1),
                new Point(currPos.R, currPos.C - 1),
            ];

            int adjSlopes = 0;
            foreach (var next in nextPoints)
            {
                if (next.R < 0 || next.C < 0) { continue; }
                if ("<>^v".Contains(input[next.R][next.C])) { adjSlopes++; }
            }

            if (adjSlopes > 1)
            {
                graphNodes.TryAdd(currPos, []);
                graphNodes[lastNode].Add(currPos, dist);
                if (!slopes)
                {
                    graphNodes[currPos].Add(lastNode, dist);
                }
                lastNode = currPos;
                dist = 0;
            }

            foreach (var next in nextPoints)
            {
                if (next.R < 0 || next.C < 0) { continue; }
                if (next == lastPos) { continue; }
                if (input[next.R][next.C] == '#') { continue; }
                if (next.R == currPos.R + 1 && input[next.R][next.C] == '^') { continue; }
                if (next.R == currPos.R - 1 && input[next.R][next.C] == 'v') { continue; }
                if (next.C == currPos.C + 1 && input[next.R][next.C] == '<') { continue; }
                if (next.C == currPos.C - 1 && input[next.R][next.C] == '>') { continue; }

                queue.Enqueue((next, currPos, lastNode, dist + 1));
            }
        }
        return graphNodes;
    }

    // Brute force solve...
    static int FindLongestPath(
        Dictionary<Point, Dictionary<Point, int>> graph,
        Point startNode,
        Point endNode)
    {
        Queue<(int dist, List<Point> seq)> queue = [];
        queue.Enqueue((0, [startNode]));
        int maxDist = 0;

        while (queue.Count > 0)
        {
            var (currDist, currSeq) = queue.Dequeue();
            var currNode = currSeq.Last();

            if (currNode == endNode)
            {
                maxDist = Math.Max(maxDist, currDist);
                continue;
            }

            foreach (var (nextNode, dist) in graph[currNode])
            {
                if (currSeq.Contains(nextNode)) { continue; }

                List<Point> nextSeq = [.. currSeq, nextNode];
                int nextDist = currDist + dist;
                queue.Enqueue((nextDist, nextSeq));
            }
        }
        return maxDist;
    }

    public override string SolvePart1(string[] input)
    {
        Point startPos = new(0, 1);
        Point endPos = new(input.Length - 1, input[0].Length - 2);
        var graph = ReduceToGraph(input, startPos, endPos);
        int ans = FindLongestPath(graph, startPos, endPos);
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        Point startPos = new(0, 1);
        Point endPos = new(input.Length - 1, input[0].Length - 2);
        var graph = ReduceToGraph(input, startPos, endPos, false);
        int ans = FindLongestPath(graph, startPos, endPos);
        return ans.ToString();
    }
}