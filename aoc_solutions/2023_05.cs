class AoC2023_05 : AoCSolution
{
    long[] seeds = [];
    readonly Dictionary<long, (long, long)>[] mapList = [[], [], [], [], [], [], []];
    readonly long[][] sortedMapSourceLBs = [[], [], [], [], [], [], []];

    void ProcessInputs(string[] input, bool reversed = false)
    {
        int currentMap = 0;
        int mapCount = mapList.Length;
        for (int line = 3; line < input.Length; line++)
        {
            if (input[line] == string.Empty)
            {
                line += 2;
                currentMap += 1;
                continue;
            }
            var mapData = input[line].Split(" ").Select(long.Parse).ToArray();
            if (reversed)
            {
                mapList[^(currentMap + 1)][mapData[0]] = (mapData[1], mapData[2]);
            }
            else
            {
                mapList[currentMap][mapData[1]] = (mapData[0], mapData[2]);
            }
        }
        seeds = input[0].Split(" ")[1..].Select(long.Parse).ToArray();

        for (int i = 0; i < mapList.Length; i++)
        {
            sortedMapSourceLBs[i] = mapList[i].Keys.Order().ToArray();
        }
        return;
    }

    (long, long) FollowMaps(long seed)
    {
        var currSourceVal = seed;
        List<long> gapsToNextRangeStart = [];
        for (int i = 0; i < mapList.Length; i++)
        {
            var map = mapList[i];
            var mapSourceLBs = sortedMapSourceLBs[i];
            for (int j = 0; j < mapSourceLBs.Length; j++)
            {
                var currSourceLB = mapSourceLBs[j];
                if (currSourceVal < currSourceLB)
                {
                    if (j == 0)
                    {
                        gapsToNextRangeStart.Add(currSourceLB - currSourceVal);
                        break;
                    }

                    var lastSourceLB = mapSourceLBs[j - 1];
                    long lastRange = map[lastSourceLB].Item2;
                    var lastSourceUB = lastSourceLB + lastRange;
                    if (currSourceVal < lastSourceUB)
                    {
                        long lastDestLB = map[lastSourceLB].Item1;
                        gapsToNextRangeStart.Add(lastSourceUB - currSourceVal);
                        currSourceVal = lastDestLB + currSourceVal - lastSourceLB;
                    }
                    else
                    {
                        gapsToNextRangeStart.Add(currSourceLB - currSourceVal);
                    }
                    break;
                }
            }
        }
        return (currSourceVal, gapsToNextRangeStart.Min());
    }


    public override string SolvePart1(string[] input)
    {
        long ans = 0;
        ProcessInputs(input);
        foreach (var seed in seeds)
        {
            var (location, _) = FollowMaps(seed);
            if (ans == 0)
            {
                ans = location;
            }
            else
            {
                ans = long.Min(ans, location);
            }
        }
        return ans.ToString();
    }


    public override string SolvePart2(string[] input)
    {
        long ans = 0;
        ProcessInputs(input, reversed: true);
        Dictionary<long, long> seedRanges = [];
        for (int i = 0; i < seeds.Length; i += 2)
        {
            seedRanges.Add(seeds[i], seeds[i + 1]);
        }
        long[] sortedSeedKeyList = seedRanges.Keys.Order().ToArray();

        while (true)
        {
            var (seed, gap) = FollowMaps(ans);
            foreach (var seedStart in sortedSeedKeyList)
            {
                if (seed >= seedStart)
                {
                    var seedEnd = seedStart + seedRanges[seedStart];
                    if (seed < seedEnd)
                    {
                        return ans.ToString();
                    }
                }
                else
                {
                    break;
                }
            }
            ans += gap;
        }
    }
}