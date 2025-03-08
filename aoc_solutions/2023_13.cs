class AoC2023_13 : AoCSolution
{
    class Pattern
    {
        public int[] rows = [];
        public int[] cols = [];

        //converts list of strings to two lists of bit masks (one for rows and columns)
        //returns new Pattern object with the data
        public static Pattern FromStringList(IList<string> strings)
        {
            int[] r = new int[strings.Count];
            int[] c = new int[strings[0].Length];
            for (int i = 0; i < strings.Count; i++)
            {
                for (int j = 0; j < strings[0].Length; j++)
                {
                    if (strings[i][j] == '#')
                    {
                        r[i] += 1 << j;
                        c[j] += 1 << i;
                    }
                }
            }
            return new() {rows = r, cols = c};
        }

        //solves part 1 along a given axis (rows or columns)
        public int SolveAxis(bool row)
        {
            int[] nums;
            int factor = 100;
            if (row) {nums = rows;}
            else {nums = cols; factor = 1;}
            
            for (int i = 1; i < nums.Length; i++)
            {
                //checks for 2 matching consecutive rows/cols
                //this marks a potential reflection line
                if (nums[i-1] != nums[i]) {continue;}
                
                int leftIdx = i-2;
                int rightIdx = i+1;
                bool trueReflection = true;
                //iterate over each outer pair of rows/cols until they dont match
                //or we reach the edge of the pattern in either direction
                while (leftIdx >= 0 && rightIdx < nums.Length)
                {
                    if (nums[leftIdx] != nums[rightIdx])
                    {
                        trueReflection = false;
                        break;
                    }
                    leftIdx--;
                    rightIdx++;
                }
                //if the pairs matched until an edge, we found a true reflection!
                if (trueReflection)
                {
                    return factor * i;
                }
            }
            return 0;
        }

        //solves part 2 along a given axis (rows or columns)
        public int SolveAxisSmudged(bool row)
        {
            int[] nums;
            int factor = 100;
            if (row) {nums = rows;}
            else {nums = cols; factor = 1;}

            //basically the same as 1
            for (int i = 1; i < nums.Length; i++)
            {
                //except we now check whether the row/col pairs differ by 1 element (bit)
                //can be done with an XOR...
                int bitDiff = nums[i - 1] ^ nums[i];
                //then checking if result is a power of 2
                bool bitDiffPowerOf2 = (bitDiff & (bitDiff - 1)) == 0;
                //we only want to find the reflection line where exactly 1 pair differs by 1 element
                bool smudgeUsed = false;

                //same as part 1 except...
                if (bitDiff != 0)
                {
                    //we can also accept a 1 element difference
                    if (!bitDiffPowerOf2) {continue;}
                    //but it uses up our smudge
                    smudgeUsed = true;
                }

                int leftIdx = i-2;
                int rightIdx = i+1;
                bool trueReflection = true;
                while (leftIdx >= 0 && rightIdx < nums.Length)
                {
                    //same as outer loop
                    int outerBitDiff = nums[leftIdx] ^ nums[rightIdx];
                    bool outerBitDiffPowerOf2 = (outerBitDiff & (outerBitDiff - 1)) == 0;

                    if (outerBitDiff != 0)
                    {
                        //if we used our smudge in the outer loop or a prior iteration...
                        //then any difference is unacceptable
                        if (smudgeUsed || !outerBitDiffPowerOf2) {trueReflection = false; break;}
                        //if we havent used our smudge yet, we can accept it but only once
                        if (!smudgeUsed && outerBitDiffPowerOf2) {smudgeUsed = true;}
                    }
                    leftIdx--;
                    rightIdx++;
                }
                //we want the case where the smudge was used but only once
                //if we didnt use the smudge, we found the part 1 answer
                if (trueReflection && smudgeUsed)
                {
                    return factor * i;
                }
            }
            return 0;
        }
        
        public int SolvePattern(bool smudged)
        {
            Func<bool, int> axisSolver;
            //chooses the right solver based on part 1 or 2
            if (smudged) {axisSolver = SolveAxisSmudged;}
            else {axisSolver = SolveAxis;}

            //checks rows first and then columns if necessary (order is arbitrary)
            int rowResult = axisSolver(true);
            if (rowResult != 0) {return rowResult;}
            return axisSolver(false);
        }
    }

    public string SolveAllPatterns(string[] input, bool smudged)
    {
        int ans = 0;
        int startIdx = 0;
        int endIdx = 0;
        bool canSolvePattern = false;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == "") {endIdx = i; canSolvePattern = true;}
            if (i == input.Length - 1) {endIdx = i+1; canSolvePattern = true;}

            if (!canSolvePattern) {continue;}
            
            ans += Pattern.FromStringList(input[startIdx..endIdx]).SolvePattern(smudged);
            startIdx = i+1;
            canSolvePattern = false;
        }
        return ans.ToString();
    }

    public override string SolvePart1(string[] input)
    {
        return SolveAllPatterns(input, smudged: false);
    }

    public override string SolvePart2(string[] input)
    {
        return SolveAllPatterns(input, smudged: true);
    }

    //function to display pattern with reflection line for debugging
    static void PrintPatternWithReflectionLine(List<string> stringList, bool row, int pos)
    {
        if (row)
        {
            stringList.Insert(pos, new string('-', stringList[0].Length));
        }

        for (int i = 0; i < stringList.Count; i++)
        {
            if (!row)
            {
                string preSplit = stringList[i][..pos];
                string postSplit = stringList[i][pos..];
                string rowWithSplit = preSplit + "|" + postSplit;
                Console.WriteLine(rowWithSplit);
            }
            else {Console.WriteLine(stringList[i]);}
        }
    }
} 