using System.Numerics;

class AoC2023_07 : AoCSolution
{
    static readonly Dictionary<string, int> baseHandValue = new()
    {
        {"FiveOf", 6},
        {"FourOf", 5},
        {"FullHouse", 4},
        {"ThreeOf", 3},
        {"TwoPair", 2},
        {"Pair", 1},
        {"HighCard", 0},
    };

    static readonly Dictionary<char, int> baseCardValue = new()
    {
        {'A', 14},
        {'K', 13},
        {'Q', 12},
        {'J', 11},
        {'T', 10},
        {'9', 9},
        {'8', 8},
        {'7', 7},
        {'6', 6},
        {'5', 5},
        {'4', 4},
        {'3', 3},
        {'2', 2},
    };

    static readonly Dictionary<char, int> baseCardValueWithJokers = new()
    {
        {'A', 14},
        {'K', 13},
        {'Q', 12},
        {'T', 10},
        {'9', 9},
        {'8', 8},
        {'7', 7},
        {'6', 6},
        {'5', 5},
        {'4', 4},
        {'3', 3},
        {'2', 2},
        {'J', 1},
    };

    (string, int)[] hands = [];

    void ProcessInputs(string[] input)
    {
        var handCards = input.Select(x => x[..5]);
        var handBids = input.Select(x => int.Parse(x[6..]));
        hands = handCards.Zip(handBids, (h, b) => (h, b)).ToArray();
        return;
    }

    static long EvaluateHand(string hand, bool usingJokers = false)
    {
        var cardValue = baseCardValue;
        if (usingJokers)
        {
            cardValue = baseCardValueWithJokers;
        }

        long handValue = 0;
        Dictionary<char, int> cardsInHand = [];
        int jokerCount = 0;
        for (int i = 0; i < hand.Length; i++)
        {
            if (usingJokers && hand[i] == 'J')
            {
                jokerCount += 1;
                continue;
            }
            if (!cardsInHand.TryAdd(hand[i], 1))
            {
                cardsInHand[hand[i]] += 1;
            }
            handValue += (long)BigInteger.Pow(10, 8 - 2 * i) * cardValue[hand[i]];
        }

        int maxMatches = 0;
        if (cardsInHand.Count > 0)
        {
            maxMatches = cardsInHand.Values.Max();
        }
        maxMatches += jokerCount;

        switch (maxMatches)
        {
            case 5:
                handValue += 10_000_000_000 * baseHandValue["FiveOf"];
                break;
            case 4:
                handValue += 10_000_000_000 * baseHandValue["FourOf"];
                break;
            case 3:
                if (cardsInHand.Count == 2)
                {
                    handValue += 10_000_000_000 * baseHandValue["FullHouse"];
                }
                else
                {
                    handValue += 10_000_000_000 * baseHandValue["ThreeOf"];
                }
                break;
            case 2:
                if (cardsInHand.Count == 3)
                {
                    handValue += 10_000_000_000 * baseHandValue["TwoPair"];
                }
                else
                {
                    handValue += 10_000_000_000 * baseHandValue["Pair"];
                }
                break;
            case 1:
                handValue += 10_000_000_000 * baseHandValue["HighCard"];
                break;
        }
        return handValue;
    }

    public override string SolvePart1(string[] input)
    {
        ProcessInputs(input);
        int ans = 0;
        hands = hands.OrderBy(x => EvaluateHand(x.Item1)).ToArray();
        for (int i = 0; i < hands.Length; i++)
        {
            ans += hands[i].Item2 * (i + 1);
        }
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        ProcessInputs(input);
        int ans = 0;
        hands = hands.OrderBy(x => EvaluateHand(x.Item1, true)).ToArray();
        for (int i = 0; i < hands.Length; i++)
        {
            ans += hands[i].Item2 * (i + 1);
        }
        return ans.ToString();
    }
}