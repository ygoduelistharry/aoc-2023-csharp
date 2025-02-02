class AoC2023_09 : AoCSolution
{
    int[][] histories = [];

    void ProcessInputs(string[] input)
    {
        static int[] SplitAndParseInt(string x)
        {
            return [.. x.Split(" ").Select(int.Parse)];
        }

        histories = [.. input.Select(SplitAndParseInt)];
    }

    static int FindNextNumber(int[] seq, bool previous = false)
    {
        if (previous)
        {
            seq = [.. seq.Reverse()];
        }

        int[] currSeq = seq;
        List<int> lastDiffs = [];
        while (currSeq.Sum() != 0)
        {
            int[] nextSeq = new int[currSeq.Length - 1];
            for (int i = 0; i < nextSeq.Length; i++)
            {
                nextSeq[i] = currSeq[i + 1] - currSeq[i];
            }
            lastDiffs.Add(nextSeq[^1]);
            currSeq = nextSeq;
        }
        return seq[^1] + lastDiffs.Sum();
    }

    public override string SolvePart1(string[] input)
    {
        ProcessInputs(input);
        int ans = histories.Aggregate(0, (a, x) => a + FindNextNumber(x));
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        ProcessInputs(input);
        int ans = histories.Aggregate(0, (a, x) => a + FindNextNumber(x, true));
        return ans.ToString();
    }
}