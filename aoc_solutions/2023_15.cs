using System.Text;

class AoC2023_15 : AoCSolution
{
    public override string SolvePart1(string[] input)
    {
        int ans = 0;
        int currVal = 0;
        foreach (byte c in Encoding.ASCII.GetBytes(input[0]))
        {
            // 44 is the comma char
            if (c == 44)
            {
                ans += currVal;
                currVal = 0;
                continue;
            }
            currVal += c;
            currVal *= 17;
            currVal %= 256;
        }
        ans += currVal;
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        int ans = 0;
        List<OrderedDictionary<string, int>> boxes = [];
        for (int i = 0; i < 256; i++) { boxes.Add([]); }
        int boxIdx = 0;
        string label = "";
        foreach (byte c in Encoding.ASCII.GetBytes(input[0]))
        {
            // comma
            if (c == 44)
            {
                boxIdx = 0;
                label = "";
                continue;
            }
            // digit 1 to 9
            if (49 <= c && c <= 57)
            {
                if (boxes[boxIdx].ContainsKey(label))
                {
                    boxes[boxIdx][label] = c - 48;
                }
                else
                {
                    boxes[boxIdx].Add(label, c - 48);
                }
                continue;
            }
            // dash
            if (c == 45)
            {
                boxes[boxIdx].Remove(label);
                continue;
            }
            // equals
            if (c == 61)
            {
                continue;
            }
            boxIdx = (boxIdx + c) * 17 % 256;
            label += Encoding.ASCII.GetChars([c])[0];
        }
        for (int i = 0; i < boxes.Count; i++)
        {
            for (int j = 0; j < boxes[i].Count; j++)
            {
                ans += (i + 1) * (j + 1) * boxes[i].GetAt(j).Value;
            }
        }
        return ans.ToString();
    }
}