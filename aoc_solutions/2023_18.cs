class AoC2023_18 : AoCSolution
{

    private static long CalculateArea(string[] input, bool useColour)
    {
        long area = 0;
        var currPos = (r: 0, c: 0);
        Dictionary<long, List<long>> nodePositions = [];
        long perimeter = 0;

        for (int i = 0; i < input.Length; i++)
        {
            string[] moveData = input[i].Split(' ');
            string colour = moveData[2];
            int deltaRow = 0;
            int deltaCol = 0;

            int dist;
            if (useColour)
            {
                dist = int.Parse(colour[2..^2], System.Globalization.NumberStyles.HexNumber);
                char dir = colour[^2];
                switch (dir)
                {
                    case '0': { deltaCol = 1; break; }
                    case '1': { deltaRow = 1; break; }
                    case '2': { deltaCol = -1; break; }
                    case '3': { deltaRow = -1; break; }
                }
            }
            else
            {
                dist = int.Parse(moveData[1]);
                char dir = moveData[0][0];
                switch (dir)
                {
                    case 'R': { deltaCol = 1; break; }
                    case 'D': { deltaRow = 1; break; }
                    case 'L': { deltaCol = -1; break; }
                    case 'U': { deltaRow = -1; break; }
                }
            }

            currPos = (currPos.r + deltaRow * dist, currPos.c + deltaCol * dist);

            if (!nodePositions.TryAdd(currPos.r, [currPos.c]))
            {
                nodePositions[currPos.r].Add(currPos.c);
            }

            perimeter += dist;
        }

        area += perimeter / 2 + 1;

        var rList = nodePositions.Keys.ToList();
        rList.Sort();

        List<long> colBounds = [];
        long currWidth = 0;
        long prevRow = 0;

        for (int r = 0; r < rList.Count; r++)
        {
            long currRow = rList[r];
            area += currWidth * (currRow - prevRow);

            foreach (int c in nodePositions[currRow].Select(v => (int)v))
            {
                if (!colBounds.Remove(c)) { colBounds.Add(c); }
            }
            colBounds.Sort();

            currWidth = 0;
            for (int c = 0; c < colBounds.Count; c++)
            {
                if (c % 2 == 0) { currWidth -= colBounds[c]; }
                else { currWidth += colBounds[c]; }
            }
            prevRow = currRow;
        }

        return area;
    }

    public override string SolvePart1(string[] input)
    {
        long ans = CalculateArea(input, useColour: false);
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        long ans = CalculateArea(input, useColour: true);
        return ans.ToString();
    }
}
