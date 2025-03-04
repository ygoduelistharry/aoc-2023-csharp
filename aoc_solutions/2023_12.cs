class AoC2023_12 : AoCSolution
{
    // defining a small class to store the sequence info we need for caching
    // also has a few useful methods to execute steps in the search algorithm
    class SequenceData
    {
        public List<(int groupsLeft, int spaceLeft)> states = [];
        public SequenceData AddDot()
        {
            SequenceData newSeqData = new()
            {
                states = this.states.ToList()
            };
            var (groupsLeft, spaceLeft) = states[^1];
            newSeqData.states.Add((groupsLeft, spaceLeft - 1));
            return newSeqData;
        }
        public SequenceData AddGroup(int count)
        {
            SequenceData newSeqData = new()
            {
                states = this.states.ToList()
            };
            var (groupsLeft, spaceLeft) = states[^1];
            newSeqData.states.Add((groupsLeft - 1, spaceLeft - count));
            return newSeqData;
        }
        //debugging function to convert the sequence to a string
        public string StringSequence()
        {
            string seq = "";
            for (int i = 1; i < states.Count; i++)
            {
                var (lastGroupsLeft, lastSpaceLeft) = states[i-1];
                var (currGroupsLeft, currSpaceLeft) = states[i];
                if (lastGroupsLeft == currGroupsLeft)
                {
                    seq += '.';
                    continue;
                }
                if (currGroupsLeft > 0)
                {
                    seq += new string('#', lastSpaceLeft - currSpaceLeft - 1);
                    seq += '.';
                    continue;
                }
                seq += new string('#', lastSpaceLeft - currSpaceLeft);
            }
            Console.WriteLine(seq);
            return seq;
        }
    }

    static long CountArrangements(string springMap, int[] springGroups)
    {
        //we are going to do depth first search with caching
        //set up the cache and define a function to map state to cache index
        long[] cache = new long[(springMap.Length + 1) * (springGroups.Length + 1)];

        int GetCacheIndex((int groupsLeft, int spaceLeft) state)
        {
            return (springGroups.Length + 1) * state.spaceLeft + state.groupsLeft;
        }
        //-1 means an unvisited state
        Array.Fill(cache, -1);

        //last element in cache repesents state where we have not placed any springs and not used any space
        //i.e. this will be the answer
        cache[^1] = 0;

        //prime the stack
        Stack<SequenceData> sequenceStack = [];
        SequenceData emptySequence = new()
        {
            states = [(springGroups.Length, springMap.Length)]
        };
        if (springMap[0] != '#')
        {
            sequenceStack.Push(emptySequence.AddDot());
        }
        if (!springMap[0..springGroups[0]].Contains('.') && springMap[springGroups[0]] != '#')
        {
            sequenceStack.Push(emptySequence.AddGroup(springGroups[0] + 1));
        }

        //do the depth first search
        while (sequenceStack.Count > 0)
        {
            //pop the latest sequence
            SequenceData currSequence = sequenceStack.Pop();
            //get the latest state
            (int groupsLeft, int spaceLeft) currState = currSequence.states[^1];

            //check the cache for count of valid sequences from this state
            int cacheIdx = GetCacheIndex(currState);
            long cacheValue = cache[cacheIdx];

            //last time we were in this state nothing was valid so continue
            if (cacheValue == 0) {continue;}
            //last time we were in this state we found valid sequences
            if (cacheValue > 0)
            {
                //so we add this number to every other state visited so far
                for (int i = 0; i < currSequence.states.Count - 1; i++)
                {
                    var state = currSequence.states[i];
                    cache[GetCacheIndex(state)] += cacheValue;
                }
                continue;
            }

            //set cache to zero to signify state has been visited
            cache[cacheIdx] = 0;
            //if we placed all the springs and there is no space left to place more
            if (currState == (0, 0))
            {
                //we found a valid sequence!
                //so we add 1 to all states visited in this sequence
                for (int i = 0; i < currSequence.states.Count; i++)
                {
                    var (groupsLeft, spaceLeft) = currSequence.states[i];
                    cache[GetCacheIndex((groupsLeft, spaceLeft))] += 1;
                }
                continue;
            }
            
            //check if adding '.' is possible
            if (currState.spaceLeft == 0) {continue;}
            int nextMapIdx = springMap.Length - currState.spaceLeft;
            if (springMap[nextMapIdx] != '#') {sequenceStack.Push(currSequence.AddDot());}

            //check if adding next group of '#' is possible
            if (currState.groupsLeft == 0) {continue;}
            int nextGroupSize = springGroups[^currState.groupsLeft];
            //need to treat the last spring group differently as it doesn't need a '.' after it
            if (currState.groupsLeft == 1)
            {
                if (nextGroupSize > currState.spaceLeft) {continue;}
                if (springMap[nextMapIdx..(nextMapIdx + nextGroupSize)].Contains('.')) {continue;}
                sequenceStack.Push(currSequence.AddGroup(nextGroupSize));
            }
            if (currState.groupsLeft > 1)
            {
                if (nextGroupSize + 1 > currState.spaceLeft) {continue;}
                if (springMap[nextMapIdx..(nextMapIdx + nextGroupSize)].Contains('.')) {continue;}
                if (springMap[nextMapIdx + nextGroupSize] == '#') {continue;}
                sequenceStack.Push(currSequence.AddGroup(nextGroupSize + 1));
            }
        }
        return cache[^1];
    }

    public override string SolvePart1(string[] input)
    {
        long ans = 0;
        foreach ((int index, string record) in input.Index())
        {
            string[] splitRecord = record.Split([',',' ']).ToArray();
            string springMap = splitRecord[0];
            int[] springGroups = splitRecord[1..].Select(int.Parse).ToArray();
            ans += CountArrangements(springMap, springGroups);
        }
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        long ans = 0;        
        foreach ((int recordIdx, string record) in input.Index())
        {
            string[] splitRecord = record.Split([',',' ']).ToArray();
            string springMap = splitRecord[0];
            int[] springGroups = splitRecord[1..].Select(int.Parse).ToArray();

            string unfoldedSpringMap = springMap;
            int[] unfoldedSpringGroups = springGroups;
            for (int i = 0; i < 4; i++)
            {
                unfoldedSpringMap += '?' + springMap;
                unfoldedSpringGroups = unfoldedSpringGroups.Concat(springGroups).ToArray();
            }
            ans += CountArrangements(unfoldedSpringMap, unfoldedSpringGroups);
        }
        return ans.ToString();
    }
}