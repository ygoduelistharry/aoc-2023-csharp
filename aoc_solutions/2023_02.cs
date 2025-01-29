class AoC2023_02 : AoCSolution
{

    readonly Dictionary<string, int> cubes = new()
    {
        ["r"] = 12,
        ["g"] = 13,
        ["b"] = 14,
    };

    public override string SolvePart1(string[] input)
    {
        // Starting answer is the sum of all game numbers.
        // We will subtract invalid games from this to get our answer.
        int ans = 101 * 50;

        foreach (string line in input)
        {
            string[] words = line.Split(" ");
            int game_number = int.Parse(words[1][..^1]);
            for (int i = 2; i < words.Length; i += 2)
            {
                int count = int.Parse(words[i]);
                string colour = words[i + 1][..1];
                if (count > cubes[colour])
                {
                    ans -= game_number;
                    break;
                }
            }
        }
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        int ans = 0;

        foreach (string line in input)
        {
            Dictionary<string, int> max_cubes = new()
            {
                ["r"] = 0,
                ["g"] = 0,
                ["b"] = 0,
            };

            string[] words = line.Split(" ");

            for (int i = 2; i < words.Length; i += 2)
            {
                int count = int.Parse(words[i]);
                string colour = words[i + 1][..1];
                if (count > max_cubes[colour])
                {
                    max_cubes[colour] = count;
                }
            }
            ans += max_cubes["r"] * max_cubes["g"] * max_cubes["b"];
        }
        return ans.ToString();
    }
}