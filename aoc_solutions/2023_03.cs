using System.Text.RegularExpressions;

class AoC2023_03 : AoCSolution
{

    readonly (int, int)[] firstDigitRelPositions = [
        (-1,-1),
        (0,-1),
        (1,-1),
    ];
    readonly (int, int)[] allDigitRelPositions = [
        (-1,0),
        (1,0),
    ];
    readonly (int, int)[] lastDigitRelPositions = [
        (-1,1),
        (0,1),
        (1,1),
    ];

    List<int> validParts = [];
    Dictionary<(int, int), List<int>> gearPairs = [];

    static (bool, string) PositionHasSymbol(string[] input, int row_num, int col_num)
    {
        int row_count = input.Length;
        int col_count = input[0].Length;

        if (row_num >= row_count || col_num >= col_count || row_num < 0 || col_num < 0)
        {
            return (false, ".");
        }

        string symbol = input[row_num][col_num].ToString();
        if (symbol == "." || int.TryParse(symbol, out int _))
        {
            return (false, symbol);
        }

        return (true, symbol);
    }

    void ProcessInputs(string[] input)
    {
        foreach ((int r, string line) in input.Index())
        {
            var matches = Regex.Matches(line, @"(\d+)");
            foreach (Match match in matches)
            {
                string numberString = match.Value;
                bool isValid = false;
                for (int i = 0; i < numberString.Length; i++)
                {
                    List<(int, int)> digitRelVecs = [.. allDigitRelPositions];
                    if (i == numberString.Length - 1)
                    {
                        digitRelVecs.AddRange(lastDigitRelPositions);
                    }
                    if (i == 0)
                    {
                        digitRelVecs.AddRange(firstDigitRelPositions);
                    }


                    int c = match.Index + i;
                    foreach ((int, int) rel in digitRelVecs)
                    {
                        (isValid, string symbol) = PositionHasSymbol(input, r + rel.Item1, c + rel.Item2);
                        if (isValid)
                        {
                            int number = int.Parse(numberString);
                            if (symbol == "*")
                            {
                                (int, int) symbolPos = (r + rel.Item1, c + rel.Item2);
                                if (!gearPairs.TryAdd(symbolPos, [number]))
                                {
                                    gearPairs[symbolPos].Add(number);
                                }
                            }
                            validParts.Add(number);
                            break;
                        }
                    }

                    if (isValid)
                    {
                        break;
                    }
                }
            }
        }
        return;
    }

    public override string SolvePart1(string[] input)
    {
        ProcessInputs(input);
        return validParts.Sum().ToString();
    }

    public override string SolvePart2(string[] input)
    {
        ProcessInputs(input);
        int ans = 0;
        foreach (List<int> gearPair in gearPairs.Values)
        {
            if (gearPair.Count == 2)
            {
                ans += gearPair[0] * gearPair[1];
            }
        }
        return ans.ToString();
    }
}