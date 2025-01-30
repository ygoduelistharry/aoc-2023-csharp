using System.Text.RegularExpressions;

class AoC2023_04 : AoCSolution
{
    static readonly IEnumerable<int> winningNumberRange = Enumerable.Range(1, 10);
    static readonly IEnumerable<int> gameNumberRange = Enumerable.Range(11, 25);

    static int CountMatches(string game)
    {
        int matches = 0;
        var numberStrings = Regex.Matches(game, @"\d+");
        HashSet<string> gameNumberStrings = [];

        foreach (int i in gameNumberRange)
        {
            gameNumberStrings.Add(numberStrings[i].Value);
        }
        foreach (int i in winningNumberRange)
        {
            if (gameNumberStrings.Contains(numberStrings[i].Value))
            {
                matches += 1;
            }
        }
        return matches;
    }

    public override string SolvePart1(string[] input)
    {
        int ans = 0;
        foreach (string game in input)
        {
            int matches = CountMatches(game);
            if (matches > 0)
            {
                ans += 1 << (matches - 1);
            }
        }
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        int[] ownedCards = [.. Enumerable.Repeat(1, input.Length)];

        for (int i = 0; i < ownedCards.Length; i++)
        {
            int matches = CountMatches(input[i]);
            for (int j = 1; j <= matches; j++)
            {
                ownedCards[i + j] += ownedCards[i];
            }
        }
        return ownedCards.Sum().ToString();
    }
}