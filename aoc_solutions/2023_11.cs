class AoC2023_11 : AoCSolution
{
    int[] emptyRows = [];
    int[] emptyCols = [];
    SortedSet<(int, int)> positions = [];

    void ProcessInputs(string[] input)
    {
        emptyRows = Enumerable.Repeat(1, input.Length).ToArray();
        emptyCols = Enumerable.Repeat(1, input[0].Length).ToArray();
        positions.Clear();
        
        for (int i = 0; i < input.Length; i++)
        {

            for (int j = 0; j < input[0].Length; j++)
            {
                char symbol = input[i][j];
                if (symbol == '#')
                {
                    positions.Add((i,j));
                    emptyRows[i] = 0;
                    emptyCols[j] = 0;
                }
            }
        }
    }

    long Distance((int, int) a, (int, int) b, int expFactor)
    {
        int ar = int.Min(a.Item1, b.Item1);
        int ac = int.Min(a.Item2, b.Item2);
        int br = int.Max(a.Item1, b.Item1);
        int bc = int.Max(a.Item2, b.Item2);
        long exp = (expFactor - 1) * (emptyRows[ar..br].Sum() + emptyCols[ac..bc].Sum());
        return bc - ac + br - ar + exp;
    }

    long SumMinDistances(int expFactor)
    {
        long totalDistance = 0;
        int posCount = positions.Count;
        for (int i = 0; i < posCount; i++)
        {
            (int, int) pos1 = positions.First();
            positions.Remove(pos1);
            foreach ((int, int) pos2 in positions)
            {
                totalDistance += Distance(pos1, pos2, expFactor);
            }
        }
        return totalDistance;
    }

    public override string SolvePart1(string[] input)
    {
        ProcessInputs(input);
        return SumMinDistances(2).ToString();
    }

    public override string SolvePart2(string[] input)
    {
        ProcessInputs(input);
        return SumMinDistances(1_000_000).ToString();
    }
}