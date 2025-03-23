class AoC2023_08 : AoCSolution
{
    readonly Dictionary<string, (string, string)> nodes = [];
    string instructions = string.Empty;

    void ProcessInputs(string[] input)
    {
        instructions = input[0];
        for (int i = 2; i < input.Length; i++)
        {
            nodes.Add(input[i][..3], (input[i][7..10], input[i][12..15]));
        }
        return;
    }

    static long GCD(long a, long b)
    {
        if (b > a) { (a, b) = (b, a); }

        var (_, rem) = Math.DivRem(a, b);
        while (rem != 0)
        {
            a = b;
            b = rem;
            (_, rem) = Math.DivRem(a, b);
        }
        return b;
    }

    static long LCM(long a, long b) { return a * b / GCD(a, b); }

    static long LCM(ICollection<long> numbers) { return numbers.Aggregate(LCM); }

    public override string SolvePart1(string[] input)
    {
        ProcessInputs(input);

        int step = 0;
        string currNode = "AAA";
        while (currNode != "ZZZ")
        {
            char instruction = instructions[step % instructions.Length];
            if (instruction == 'L')
            {
                currNode = nodes[currNode].Item1;
            }
            else
            {
                currNode = nodes[currNode].Item2;
            }
            step += 1;
        }
        return step.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        ProcessInputs(input);

        string[] startNodes = nodes.Keys.Where(x => x.Last() == 'A').ToArray();
        List<int> cycles = [];

        foreach (string node in startNodes)
        {
            int step = 0;
            string currNode = node;
            while (currNode.Last() != 'Z')
            {
                char instruction = instructions[step % instructions.Length];
                if (instruction == 'L')
                {
                    currNode = nodes[currNode].Item1;
                }
                else
                {
                    currNode = nodes[currNode].Item2;
                }

                step += 1;
            }
            cycles.Add(step);
        }

        return LCM(cycles.ConvertAll(n => (long)n)).ToString();
    }
}