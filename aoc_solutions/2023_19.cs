using System.Text.RegularExpressions;

partial class AoC2023_19 : AoCSolution
{
    [GeneratedRegex(@"\d+")]
    private static partial Regex FindNumbers();

    partial record struct Part(int X, int M, int A, int S)
    {
        public static Part StringToPart(string p)
        {
            var vals = FindNumbers().Matches(p).ToArray();
            Part part = new(
                X: int.Parse(vals[0].ToString()),
                M: int.Parse(vals[1].ToString()),
                A: int.Parse(vals[2].ToString()),
                S: int.Parse(vals[3].ToString())
            );
            return part;
        }

        public readonly int? GetValue(char? cat)
        {
            switch (cat)
            {
                case 'x': { return X; }
                case 'm': { return M; }
                case 'a': { return A; }
                case 's': { return S; }
            }
            return null;
        }

        public void SetValue(char? cat, int val)
        {
            switch (cat)
            {
                case 'x': { X = val; break; }
                case 'm': { M = val; break; }
                case 'a': { A = val; break; }
                case 's': { S = val; break; }
            }
            return;
        }

        public readonly int Sum()
        {
            return X + M + A + S;
        }
    }

    record struct Rule(char? Cat, char? Comp, int? Val, string Dest)
    {
        public static Rule StringToRule(string r)
        {
            string[] split = r.Split(':');
            Rule rule = new(
                Cat: r[0],
                Comp: r[1],
                Val: int.Parse(split[0][2..]),
                Dest: split[1]
            );
            return rule;
        }

        public readonly string? ApplyRule(Part part)
        {
            if (Cat is null) { return Dest; }

            int partVal = part.GetValue((char)Cat) ?? 0;

            if (Comp == '<' && partVal < Val) { return Dest; }
            if (Comp == '>' && partVal > Val) { return Dest; }

            return null;
        }
    }

    readonly Dictionary<string, List<Rule>> workflows = [];
    readonly List<Part> parts = [];

    private void ProcessInputs(string[] input)
    {
        foreach (string line in input)
        {
            if (line == "") { continue; }
            if (line[0] == '{')
            {
                parts.Add(Part.StringToPart(line));
            }
            else
            {
                AddWorkflow(line);
            }
        }
    }

    void AddWorkflow(string wf)
    {
        string[] split = wf.Split(['{', '}', ',']);

        string workflowName = split[0];

        List<Rule> rules = [];
        for (int i = 1; i < split.Length - 2; i++)
        {
            rules.Add(Rule.StringToRule(split[i]));
        }
        Rule lastRule = new(null, null, null, split[^2]);
        rules.Add(lastRule);

        workflows.TryAdd(workflowName, rules);
    }

    static (bool, int) ProcessPart(Dictionary<string, List<Rule>> workflows, Part part)
    {
        string currWorkflow = "in";
        while (true)
        {
            foreach (var rule in workflows[currWorkflow])
            {
                string? nextWorkflow = rule.ApplyRule(part);
                switch (nextWorkflow)
                {
                    case null: { continue; }
                    case "A": { return (true, part.Sum()); }
                    case "R": { return (false, part.Sum()); }
                }
                currWorkflow = nextWorkflow;
                break;
            }
        }
    }

    public override string SolvePart1(string[] input)
    {
        int ans = 0;
        ProcessInputs(input);
        foreach (Part part in parts)
        {
            (bool accepted, int value) = ProcessPart(workflows, part);
            if (accepted) { ans += value; }
        }
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        long ans = 0;
        ProcessInputs(input);
        Stack<(string wf, Part min, Part max)> stack = [];

        Part minPart = new(1, 1, 1, 1);
        Part maxPart = new(4000, 4000, 4000, 4000);
        stack.Push(("in", minPart, maxPart));

        while (stack.Count > 0)
        {
            var (currWf, currMin, currMax) = stack.Pop();

            if (currWf == "R") { continue; }
            if (currWf == "A")
            {
                long xr = currMax.X - currMin.X + 1;
                long mr = currMax.M - currMin.M + 1;
                long ar = currMax.A - currMin.A + 1;
                long sr = currMax.S - currMin.S + 1;
                ans += xr * mr * ar * sr;
                continue;
            }

            var currRules = workflows[currWf];
            foreach (var rule in currRules)
            {
                if (rule.Val is null)
                {
                    stack.Push((rule.Dest, currMin, currMax));
                    break;
                }

                var nextMin = currMin;
                var nextMax = currMax;
                if (rule.Comp == '<')
                {
                    nextMax.SetValue(rule.Cat, (int)rule.Val - 1);
                    nextMin.SetValue(rule.Cat, (int)rule.Val);
                    stack.Push((rule.Dest, currMin, nextMax));
                    currMin = nextMin;
                }
                if (rule.Comp == '>')
                {
                    nextMax.SetValue(rule.Cat, (int)rule.Val);
                    nextMin.SetValue(rule.Cat, (int)rule.Val + 1);
                    stack.Push((rule.Dest, nextMin, currMax));
                    currMax = nextMax;
                }
            }
        }

        return ans.ToString();
    }
}