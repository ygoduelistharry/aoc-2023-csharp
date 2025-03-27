class AoC2023_21 : AoCSolution
{

    static Dictionary<(int r, int c), int> BFS(string[] input, (int r, int c)? startPos)
    {
        int rowCount = input.Length;
        int colCount = input[0].Length;

        Dictionary<(int r, int c), int> distanceMap = [];
        Queue<(int r, int c)> queue = [];

        if (startPos is not null)
        {
            distanceMap.Add(((int, int))startPos, 0);
            queue.Enqueue(((int, int))startPos);
        }
        else
        {
            bool startFound = false;
            for (int r = 0; r < input.Length; r++)
            {
                for (int c = 0; c < input[0].Length; c++)
                {
                    if (input[r][c] == 'S')
                    {
                        startFound = true;
                        distanceMap.Add((r, c), 0);
                        queue.Enqueue((r, c));
                        break;
                    }
                }
                if (startFound) { break; }
            }
        }

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();
            int dist = distanceMap[(r, c)];

            List<(int r, int c)> nextPoints = [];
            nextPoints.Add((r + 1, c));
            nextPoints.Add((r - 1, c));
            nextPoints.Add((r, c + 1));
            nextPoints.Add((r, c - 1));

            foreach (var (nextR, nextC) in nextPoints)
            {
                if (nextR < 0 || nextC < 0 || nextR >= rowCount || nextC >= colCount)
                {
                    continue;
                }
                if (input[nextR][nextC] != '#')
                {
                    if (distanceMap.TryAdd((nextR, nextC), dist + 1))
                    {
                        queue.Enqueue((nextR, nextC));
                    }
                    continue;
                }
            }
        }
        return distanceMap;
    }

    public override string SolvePart1(string[] input)
    {
        Dictionary<(int r, int c), int> startPointDistanceMap = BFS(input, null);
        int ans = 0;
        foreach (var (point, dist) in startPointDistanceMap)
        {
            // because we can retrace our steps, we notice that each point within
            // range will either be reachable or unreachable depending on whether
            // we take an odd or even total steps.
            // also, two adjacent points CANT both be reachable simultaneously.

            // because 64 is even, we can count all points within range which
            // have even distance.
            if (dist <= 64 && dist % 2 == 0) { ans++; }
        }
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        // we notice two properties of the input;
        // 1. there is a blank area which looks like a diamond containing no obstacles
        // 2. the horizontal and vertical rows with the start point have no obstacles
        // 3. the perimiter of the area has no obstacles

        // finally we notice that the steps required is (202300 * size of grid) + 65
        // where size of grid = 131

        // 65 happens to be the number of steps it takes to get from;
        // 1. the start point to every point on the inscribed diamond
        // 2. any corner to its closest inscribed diamond edge (confirmed visually)

        // using these properties, we can solve the problem relatively easily.

        // we know from the start point if we head in any of the cardinal directions we will
        // 1. traverse to the edge of the first grid (65 steps)
        // 2. traverse an extra 202300 full grids lengths

        // the traversal pattern will be a diamond expanding from the start point, therefore:
        // in all 202301st grids we wont traverse the full grid as we cant reach some corners
        // however, we will also be able to traverse some corners of the 202302nd grids.

        // the same "polarity" rules from part 1 apply. the total step count is odd, BUT
        // because the grid is odd sized (131 x 131), the number of steps required to reach
        // the next grid will alternate between odd and even.
        // for example;
        // fully exploring the 1st grid takes 131 steps so it will be "odd" at this time.
        // fully exploring the 2nd grid takes an additional 131 steps so it will be "odd".
        // HOWEVER the total steps taken would be 262 so the 1st grid will be "even".
        // therefore;
        // since the total step count is odd, the 1st grid will be "odd"
        // the 4 surrounding grids will be "even"
        // the 8 grids surrounding those out will be "odd" again, etc. etc.

        Dictionary<(int r, int c), int> startPointDistanceMap = BFS(input, null);

        // we need to calculate these 4 parameters.
        // turns out we always need a full set of 4 corners to add or subtract
        // so theres no point calculating each corner separately

        long oddFullGrid = 0;
        long evenFullGrid = 0;
        long oddCorners = 0;
        long evenCorners = 0;

        foreach (var (point, dist) in startPointDistanceMap)
        {
            if (dist % 2 == 0)
            {
                evenFullGrid++;
                if (dist > 65) { evenCorners++; }
            }
            else
            {
                oddFullGrid++;
                if (dist > 65) { oddCorners++; }
            }
        }

        // using the starting grid as grid n = 0;
        // odd grid count = (n - n%2 + 1)^2
        // even grid count = (n + n%2)^2
        // odd corners to subtract = n + 1
        // even corners to add = n
        long n = 26501365 / 131;
        // we know n = 202300, so n%2 = 0;
        long ans =
            oddFullGrid * (n + 1) * (n + 1)
            + evenFullGrid * n * n
            - oddCorners * (n + 1)
            + evenCorners * n;

        return ans.ToString();
    }
}