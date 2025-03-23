using System.Text.RegularExpressions;

class AoC2023_20 : AoCSolution
{
    const bool HIGH = true;
    const bool LOW = false;

    abstract class Node()
    {
        public List<string> outputNodes = [];
        public abstract List<(string, string, bool)> ProcessPulse((string from, string to, bool pulse) data);
    };

    class Output : Node
    {
        public override List<(string, string, bool)> ProcessPulse((string from, string to, bool pulse) data)
        {
            return [];
        }
    };

    class Broadcaster : Node
    {
        public override List<(string, string, bool)> ProcessPulse((string from, string to, bool pulse) data)
        {
            List<(string, string, bool)> nextPulses = [];
            foreach (var node in outputNodes) { nextPulses.Add((data.to, node, LOW)); }
            return nextPulses;
        }
    };

    class FlipFlop : Node
    {
        public bool on = false;

        public override List<(string, string, bool)> ProcessPulse((string from, string to, bool pulse) data)
        {
            if (data.pulse == HIGH) { return []; }
            on = !on;

            List<(string, string, bool)> nextPulses = [];
            foreach (var node in outputNodes) { nextPulses.Add((data.to, node, on)); }
            return nextPulses;
        }
    }

    class Conjunction : Node
    {
        public Dictionary<string, bool> lastInputNodePulses = [];

        public override List<(string, string, bool)> ProcessPulse((string from, string to, bool pulse) data)
        {

            lastInputNodePulses[data.from] = data.pulse;
            bool nextPulse = LOW;
            foreach (var (_, lastPulse) in lastInputNodePulses)
            {
                if (lastPulse == LOW) { nextPulse = HIGH; break; }
            }

            List<(string, string, bool)> nextPulses = [];
            foreach (var node in outputNodes) { nextPulses.Add((data.to, node, nextPulse)); }
            return nextPulses;
        }
    }

    readonly Dictionary<string, Node> nodes = [];

    void ProcessInputs(string[] input)
    {
        foreach (string n in input)
        {
            if (n == "") { continue; }
            switch (n[0])
            {
                case '%': { nodes.Add(n[1..3], new FlipFlop()); break; }
                case '&': { nodes.Add(n[1..3], new Conjunction()); break; }
                case 'b': { nodes.Add("broadcaster", new Broadcaster()); break; }
            }
        }

        foreach (string n in input)
        {
            if (n == "") { continue; }
            var split = Regex.Matches(n, @"[a-z]+");
            string inputNode = split[0].ToString();
            for (int i = 1; i < split.Count; i++)
            {
                string outputNode = split[i].ToString();
                nodes[inputNode].outputNodes.Add(outputNode);
                if (nodes.TryGetValue(outputNode, out var node))
                {
                    if (node is Conjunction cNode)
                    {
                        cNode.lastInputNodePulses.Add(inputNode, LOW);
                    }
                }
                else { nodes.Add(outputNode, new Output()); }
            }
        }
    }

    readonly Dictionary<string, int> firstLowPulse = [];
    int buttonPresses = 0;
    (long lowCount, long highCount) PressButton(int count)
    {
        long lowCount = 0;
        long highCount = 0;
        for (int i = 0; i < count; i++)
        {
            buttonPresses += 1;
            Queue<(string, string, bool)> queue = [];
            queue.Enqueue(("button", "broadcaster", LOW));
            while (queue.Count > 0)
            {
                (string from, string to, bool pulse) pulseData = queue.Dequeue();
                if (pulseData.pulse == HIGH) { highCount++; }
                else
                {
                    lowCount++;
                    firstLowPulse.TryAdd(pulseData.to, buttonPresses);
                }

                foreach (var outputPulse in nodes[pulseData.to].ProcessPulse(pulseData))
                {
                    queue.Enqueue(outputPulse);
                }
            }
        }
        return (lowCount, highCount);
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
        var (low, high) = PressButton(1000);
        long ans = low * high;
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        ProcessInputs(input);
        HashSet<string> finalNodes = [];
        List<long> finalNodePeriods = [];
        foreach (var (k, v) in nodes)
        {
            if (v.outputNodes.Contains("rx") && nodes[k] is Conjunction finalConNode)
            {
                finalNodes = finalConNode.lastInputNodePulses.Keys.ToHashSet();
                break;
            }
        }

        while (finalNodes.Count > 0)
        {
            PressButton(1);
            foreach (string node in finalNodes)
            {
                if (firstLowPulse.TryGetValue(node, out int period))
                {
                    finalNodes.Remove(node);
                    finalNodePeriods.Add(period);
                }
            }
        }
        long ans = LCM(finalNodePeriods);
        return ans.ToString();
    }
}