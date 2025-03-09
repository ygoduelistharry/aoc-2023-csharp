class AoC2023_16 : AoCSolution
{
    const int RIGHT = 0;
    const int DOWN = 1;
    const int LEFT = 2;
    const int UP = 3;

    private static int BFS(string[] input, (int row, int col, int dir) initialState)
    {
        Queue<(int row, int col, int dir)> stateQueue = [];
        HashSet<(int row, int col, int dir)> seenStates = [];
        int rowCount = input.Length;
        int colCount = input[0].Length;

        stateQueue.Enqueue(initialState);
        seenStates.Add(initialState);

        while (stateQueue.Count > 0)
        {
            var (currRow, currCol, currDir) = stateQueue.Dequeue();
            int nextRow = currRow;
            int nextCol = currCol;
            int nextDir = currDir;
            int nextRow2 = currRow;
            int nextCol2 = currCol;
            int nextDir2 = currDir;
            bool splitBeam = false;
            char currSymbol = input[currRow][currCol];
            switch (currSymbol, currDir)
            {
                // left turn is -1 to dir, right turn is +1 to dir
                case ('/', RIGHT or LEFT): { nextDir = (nextDir + 3) % 4; break; }
                case ('/', DOWN or UP): { nextDir = (nextDir + 5) % 4; break; }
                case ('\\', RIGHT or LEFT): { nextDir = (nextDir + 5) % 4; break; }
                case ('\\', DOWN or UP): { nextDir = (nextDir + 3) % 4; break; }
                case ('|', RIGHT or LEFT):
                    {
                        nextDir = DOWN;
                        nextDir2 = UP;
                        nextRow2 -= 1;
                        splitBeam = true;
                        break;
                    }
                case ('-', DOWN or UP):
                    {
                        nextDir = RIGHT;
                        nextDir2 = LEFT;
                        nextCol2 -= 1;
                        splitBeam = true;
                        break;
                    }
            }

            switch (nextDir)
            {
                case RIGHT: { nextCol += 1; break; }
                case DOWN: { nextRow += 1; break; }
                case LEFT: { nextCol -= 1; break; }
                case UP: { nextRow -= 1; break; }
            }

            if (nextCol >= 0 && nextRow >= 0 && nextCol < colCount && nextRow < rowCount)
            {
                var nextState = (nextRow, nextCol, nextDir);
                if (!seenStates.Contains(nextState))
                {
                    stateQueue.Enqueue(nextState);
                    seenStates.Add(nextState);
                }
            }
            if (!splitBeam) { continue; }

            if (nextCol2 >= 0 && nextRow2 >= 0 && nextCol2 < colCount && nextRow2 < rowCount)
            {
                var nextState2 = (nextRow2, nextCol2, nextDir2);
                if (!seenStates.Contains(nextState2))
                {
                    stateQueue.Enqueue(nextState2);
                    seenStates.Add(nextState2);
                }
            }
        }

        Dictionary<(int row, int col), int> visitedTiles = [];
        foreach (var (row, col, _) in seenStates)
        {
            if (!visitedTiles.TryAdd((row, col), 1))
            {
                visitedTiles[(row, col)] += 1;
            }
        }
        return visitedTiles.Count;
    }

    public override string SolvePart1(string[] input)
    {
        int ans = BFS(input, (0, 0, RIGHT));
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        int ans = 0;
        for (int i = 0; i < input.Length; i++)
        {
            ans = Math.Max(ans, BFS(input, (i, 0, RIGHT)));
            ans = Math.Max(ans, BFS(input, (i, input[0].Length - 1, LEFT)));
        }
        for (int i = 0; i < input[0].Length; i++)
        {
            ans = Math.Max(ans, BFS(input, (0, i, DOWN)));
            ans = Math.Max(ans, BFS(input, (input.Length - 1, i, UP)));
        }
        return ans.ToString();
    }
}