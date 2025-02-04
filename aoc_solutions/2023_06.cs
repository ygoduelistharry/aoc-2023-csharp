class AoC2023_06 : AoCSolution
{
    long[] raceDurations = [];
    long[] records = [];

    void ProcessInputs(string[] input)
    {
        raceDurations =
            input[0]
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)[1..5]
                .Select(long.Parse)
                .ToArray()
            ;

        records =
            input[1]
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)[1..5]
                .Select(long.Parse)
                .ToArray()
            ;

        return;
    }

    static long WinningRange(long tMax, long r)
    {
        double d = Math.Sqrt(Math.Pow(tMax, 2) - 4 * r);
        float t1 = (float)((tMax - d) / 2);
        float t2 = (float)((tMax + d) / 2);
        return (long)(Math.Ceiling(t2) - Math.Ceiling(t1));
    }

    public override string SolvePart1(string[] input)
    {
        ProcessInputs(input);
        long ans = 1;
        for (int i = 0; i < raceDurations.Length; i++)
        {
            ans *= WinningRange(raceDurations[i], records[i]);
        }
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        ProcessInputs(input);
        var raceDurationStrings = raceDurations.Select(x => x.ToString()).ToArray();
        var recordStrings = records.Select(x => x.ToString()).ToArray();

        var raceDuration = long.Parse(string.Join("", raceDurationStrings));
        var record = long.Parse(string.Join("", recordStrings));

        return WinningRange(raceDuration, record).ToString();
    }
}