using System.Text.RegularExpressions;

class AoC2023_25 : AoCSolution
{
    static Dictionary<string, List<string>> ProcessInputs(string[] input)
    {
        Dictionary<string, List<string>> nodes = [];
        foreach (string s in input)
        {
            var nodeList = Regex.Matches(s, @"[a-z]+");
            string root = nodeList[0].ToString();
            for (int i = 1; i < nodeList.Count; i++)
            {
                string other = nodeList[i].ToString();
                if (!nodes.TryAdd(root, [other]))
                {
                    nodes[root].Add(other);
                }
                if (!nodes.TryAdd(other, [root]))
                {
                    nodes[other].Add(root);
                }
            }
        }
        return nodes;
    }

    static (Dictionary<string, int>, HashSet<string>) CountEdgeVisits(Dictionary<string, List<string>> nodes, string start)
    {
        Dictionary<string, int> visits = [];
        Queue<(string, List<string>)> queue = [];
        queue.Enqueue((start, []));
        HashSet<string> seenNodes = [start];

        while (queue.Count() > 0)
        {
            var (currNode, edges) = queue.Dequeue();
            seenNodes.Add(currNode);

            foreach (var edge in edges)
            {
                if (!visits.TryAdd(edge, 1))
                {
                    visits[edge] += 1;
                }
            }

            var nextNodes = nodes[currNode];
            foreach (var nextNode in nextNodes)
            {
                if (seenNodes.Contains(nextNode)) { continue; }
                string n1 = currNode;
                string n2 = nextNode;
                if (string.Compare(currNode, nextNode) > 0)
                {
                    n1 = nextNode;
                    n2 = currNode;
                }
                queue.Enqueue((nextNode, [.. edges, n1 + n2]));
            }
        }
        return (visits, seenNodes);
    }

    public override string SolvePart1(string[] input)
    {
        var nodes = ProcessInputs(input);
        Dictionary<string, int> allVisits = [];

        // count how many edge visits are in the shortest paths from each node to every other node
        // it's likely that the most visited edges join the groups if they are roughly equal size
        foreach (string startNode in nodes.Keys)
        {
            var (edgeVisits, _) = CountEdgeVisits(nodes, startNode);
            foreach (var (edge, count) in edgeVisits)
            {
                if (!allVisits.TryAdd(edge, count))
                {
                    allVisits[edge] += count;
                }
            }
        }

        // sort edges in descending order of visits
        List<(string edge, int count)> allVisitsList = [];
        foreach (var (edge, count) in allVisits)
        {
            allVisitsList.Add((edge, count));
        }
        var sortedVisits = allVisitsList.OrderBy(x => -x.count).ToList();

        // remove the 3 most visited edges
        foreach (var (edge, _) in sortedVisits[..3])
        {
            string n1 = edge[..3];
            string n2 = edge[3..];
            nodes[n1].Remove(n2);
            nodes[n2].Remove(n1);
        }

        // do one more bfs from any node
        var (_, group1) = CountEdgeVisits(nodes, nodes.Keys.First());

        int ans = group1.Count * (nodes.Count - group1.Count);
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        int ans = 0;
        return ans.ToString();
    }
}