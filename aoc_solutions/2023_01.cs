using System.Text.RegularExpressions;

class AoC2023_01 : AoCSolution
{

    Dictionary<string, string> digits = new()
    {
        ["one"] = "1",
        ["two"] = "2",
        ["three"] = "3",
        ["four"] = "4",
        ["five"] = "5",
        ["six"] = "6",
        ["seven"] = "7",
        ["eight"] = "8",
        ["nine"] = "9",
    };

    override public string SolvePart1(string[] input)
    {
        int ans = 0;
        foreach (string line in input)
        {
            var re = Regex.Matches(line, @"(\d)");
            string firstDigit = re[0].ToString();
            string lastDigit = re[^1].ToString();
            ans += int.Parse(firstDigit + lastDigit);
        }
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        int ans = 0;
        foreach (string line in input)
        {
            var re = Regex.Matches(line, @"(?=(\d|one|two|three|four|five|six|seven|eight|nine))");

            string firstDigit = re[0].Groups[1].ToString();
            string lastDigit = re[^1].Groups[1].ToString();

            if (digits.TryGetValue(firstDigit, out string? f)) { firstDigit = f; }
            if (digits.TryGetValue(lastDigit, out string? l)) { lastDigit = l; }

            ans += int.Parse(firstDigit + lastDigit);
        }
        return ans.ToString();
    }
}