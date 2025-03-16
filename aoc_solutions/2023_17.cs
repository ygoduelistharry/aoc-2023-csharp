class AoC2023_17 : AoCSolution
{
    const ushort RIGHT = 0;
    const ushort DOWN = 1;
    const ushort LEFT = 2;
    const ushort UP = 3;

    readonly ushort[] dirs = [UP, LEFT, DOWN, RIGHT];
    record struct State(ushort Row, ushort Col, ushort Dir, ushort Rep);

    int Djikstra(string[] input, int minRep, int maxRep)
    {
        int rowCount = input.Length;
        int colCount = input[0].Length;
        PriorityQueue<State, int> priorityQueue = new();
        State initialState = new(0, 0, 4, (ushort)minRep);
        HashSet<State> visitedStates = [];
        priorityQueue.Enqueue(initialState, 0);

        while (priorityQueue.Count > 0)
        {
            priorityQueue.TryDequeue(out State currState, out int distance);

            // check if we've seen this state before
            if (visitedStates.Contains(currState)) { continue; }

            // check if we've found the goal
            if (currState.Row == rowCount - 1 && currState.Col == colCount - 1)
            {
                return distance;
            }

            visitedStates.Add(currState);

            foreach (ushort dir in dirs)
            {
                // cant go backwards
                if (dir == UP && currState.Dir == DOWN) { continue; }
                if (dir == DOWN && currState.Dir == UP) { continue; }
                if (dir == LEFT && currState.Dir == RIGHT) { continue; }
                if (dir == RIGHT && currState.Dir == LEFT) { continue; }

                int nextRow = currState.Row;
                int nextCol = currState.Col;
                int nextRep = currState.Rep;

                switch (dir)
                {
                    case UP: { nextRow -= 1; break; }
                    case DOWN: { nextRow += 1; break; }
                    case LEFT: { nextCol -= 1; break; }
                    case RIGHT: { nextCol += 1; break; }
                }

                // check for bounds of grid
                if (nextCol < 0 || nextCol >= colCount) { continue; }
                if (nextRow < 0 || nextRow >= rowCount) { continue; }

                // check for min/max required repeated movements in a single direction
                if (dir == currState.Dir && currState.Rep >= maxRep) { continue; }
                if (dir != currState.Dir && currState.Rep < minRep) { continue; }

                if (dir != currState.Dir) { nextRep = 1; }
                else { nextRep += 1; }

                State nextState = new((ushort)nextRow, (ushort)nextCol, dir, (ushort)nextRep);
                int nextDist = distance + (ushort)char.GetNumericValue(input[nextRow][nextCol]);
                priorityQueue.Enqueue(nextState, nextDist);
            }
        }
        Console.WriteLine("Path to target not found!");
        return -1;
    }

    public override string SolvePart1(string[] input)
    {
        int ans = Djikstra(input, 1, 3);
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        int ans = Djikstra(input, 4, 10);
        return ans.ToString();
    }
}