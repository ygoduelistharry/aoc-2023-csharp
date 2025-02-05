class AoC2023_10 : AoCSolution
{
    readonly Dictionary<char, string> pipes = new()
    {
        ['R'] = "J-7",
        ['L'] = "F-L",
        ['D'] = "J|L",
        ['U'] = "7|F",
    };

    readonly Dictionary<char, int[]> relVecs = new()
    {
        ['R'] = [ 0,  1],
        ['L'] = [ 0, -1],
        ['D'] = [ 1,  0],
        ['U'] = [-1,  0],
    };


    Dictionary<(int,int), char> ProcessInputs(string[] input)
    {
        // Find the position of the S
        int[][] currPositions = [];
        int[] startPos = [];
        bool foundStart = false;
        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input[i].Length; j++)
            {
                if (input[i][j] == 'S')
                {
                    startPos = [i, j];
                    currPositions = [[i, j], [i, j]];
                    foundStart = true;
                    break;
                }
            }
            if (foundStart)
            {
                break;
            }
        }

        // Find the two directions to move to start travelling in opposite directions.
        char[] prevMoves = new char[2];
        bool firstPos = true;
        foreach (var dir in pipes.Keys)
        {
            int row = startPos[0] + relVecs[dir][0];
            int col = startPos[1] + relVecs[dir][1];
            if (pipes[dir].Contains(input[row][col]))
            {
                if (firstPos)
                {
                    currPositions[0] = [row, col];
                    prevMoves[0] = dir;
                    firstPos = false;
                }
                else
                {
                    currPositions[1] = [row, col];
                    prevMoves[1] = dir;
                    break;
                }
            }
        }

        // Create dictionary of positions of pipes on the loop and what they are.
        Dictionary<(int, int), char> loopPositions = [];
        // Need to figure out concretely what pipe the S would be.
        char sPipeType = 'S';
        if (prevMoves.Contains('D') && prevMoves.Contains('U')) {sPipeType = '|';}
        if (prevMoves.Contains('D') && prevMoves.Contains('L')) {sPipeType = '7';}
        if (prevMoves.Contains('D') && prevMoves.Contains('R')) {sPipeType = 'F';}
        if (prevMoves.Contains('U') && prevMoves.Contains('L')) {sPipeType = 'J';}
        if (prevMoves.Contains('U') && prevMoves.Contains('R')) {sPipeType = 'L';}
        if (prevMoves.Contains('L') && prevMoves.Contains('R')) {sPipeType = '-';}
        loopPositions.Add((startPos[0], startPos[1]), sPipeType);
        // Also add the loop positons after the first moves.
        var currRow0 = currPositions[0][0];
        var currCol0 = currPositions[0][1];
        var currRow1 = currPositions[1][0];
        var currCol1 = currPositions[1][1];
        loopPositions.Add((currRow0, currCol0), input[currRow0][currCol0]);
        loopPositions.Add((currRow1, currCol1), input[currRow1][currCol1]);

        int steps = 1;
        while (!currPositions[0].SequenceEqual(currPositions[1]))
        {
            for (int i = 0; i < 2; i++)
            {
                int currRow = currPositions[i][0];
                int currCol = currPositions[i][1];
                char currPipe = input[currRow][currCol];
                char nextMove = new();

                switch (prevMoves[i], currPipe)
                {
                    case ('R', '-'): nextMove = 'R'; break;
                    case ('R', '7'): nextMove = 'D'; break;
                    case ('R', 'J'): nextMove = 'U'; break;
                    case ('L', '-'): nextMove = 'L'; break;
                    case ('L', 'F'): nextMove = 'D'; break;
                    case ('L', 'L'): nextMove = 'U'; break;
                    case ('D', '|'): nextMove = 'D'; break;
                    case ('D', 'L'): nextMove = 'R'; break;
                    case ('D', 'J'): nextMove = 'L'; break;
                    case ('U', '|'): nextMove = 'U'; break;
                    case ('U', 'F'): nextMove = 'R'; break;
                    case ('U', '7'): nextMove = 'L'; break;
                }
                var nextMoveRelVec = relVecs[nextMove];
                var nextRow = currPositions[i][0] + nextMoveRelVec[0];
                var nextCol = currPositions[i][1] + nextMoveRelVec[1];
                loopPositions.TryAdd((nextRow, nextCol), input[nextRow][nextCol]);

                currPositions[i][0] = nextRow;
                currPositions[i][1] = nextCol;
                prevMoves[i] = nextMove;
            }
            steps += 1;
        }
        return loopPositions;
    }


    public override string SolvePart1(string[] input)
    {
        var loop = ProcessInputs(input);
        int ans = loop.Count >> 1;
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        int ans = 0;
        var loop = ProcessInputs(input);
        for (int row = 0; row < input.Length; row++)
        {
            bool inner = false;
            for (int col = 0; col < input[row].Length; col++)
            {
                if (loop.ContainsKey((row, col)))
                {   
                    if ("J|L".Contains(input[row][col]))
                    {
                        inner = !inner;
                    }
                    continue;
                }

                if (inner)
                {
                    ans += 1;
                }
            }
        }
        return ans.ToString();
    }
}