class AoC2023_14 : AoCSolution
{
    public override string SolvePart1(string[] input)
    {
        int[] rockVals = new int[input[0].Length];
        Array.Fill(rockVals, input.Length);

        int ans = 0;
        for (int row = 0; row < input.Length; row++)
        {
            for (int col = 0; col < input[0].Length; col++)
            {
                switch (input[row][col])
                {
                    case '.':
                        {
                            break;
                        }
                    case 'O': 
                        {
                            ans += rockVals[col]--;
                            break;
                        }
                    case '#':
                        {
                            rockVals[col] = input.Length - row - 1;
                            break;
                        }
                }
            }
        }
        return ans.ToString();
    }

    class Dish
    {
        public UInt128[] fixedBitmap = [];
        public UInt128[] rockBitmap = [];
        public int rowCount;
        public int colCount;

        public static Dish FromStringList(string[] input)
        {
            Dish dish = new()
            {
                rowCount = input.Length,
                colCount = input[0].Length,
                fixedBitmap = new UInt128[input.Length],
                rockBitmap = new UInt128[input.Length],
            };

            for (int r = 0; r < input.Length; r++)
            {
                for (int c = 0; c < input[0].Length; c++)
                {
                    switch (input[r][c])
                    {
                        case '.':
                            {
                                break;
                            }
                        case 'O': 
                            {
                                dish.rockBitmap[r] += (UInt128)1 << c;
                                break;
                            }
                        case '#':
                            {
                                dish.fixedBitmap[r] += (UInt128)1 << c;
                                break;
                            }
                    }
                }
            }
            return dish;
        }

        public void TiltNorth()
        {
            int[] nextValidPos = new int[colCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if ((fixedBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        nextValidPos[j] = i + 1;
                    }
                    if ((rockBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        rockBitmap[i] -= (UInt128)1 << j;
                        rockBitmap[nextValidPos[j]] += (UInt128)1 << j;
                        nextValidPos[j]++;
                    }
                }
            }
            return;
        }

        public void TiltSouth()
        {
            int[] nextValidPos = new int[colCount];
            Array.Fill(nextValidPos, rowCount - 1);
            for (int i = rowCount - 1; i >= 0; i--)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if ((fixedBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        nextValidPos[j] = i - 1;
                    }
                    if ((rockBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        rockBitmap[i] -= (UInt128)1 << j;
                        rockBitmap[nextValidPos[j]] += (UInt128)1 << j;
                        nextValidPos[j]--;
                    }
                }
            }
            return;
        }

        public void TiltWest()
        {
            int[] nextValidPos = new int[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if ((fixedBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        nextValidPos[i] = j + 1;
                    }
                    if ((rockBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        rockBitmap[i] -= (UInt128)1 << j;
                        rockBitmap[i] += (UInt128)1 << nextValidPos[i];
                        nextValidPos[i]++;
                    }
                }
            }
            return;
        }

        public void TiltEast()
        {
            int[] nextValidPos = new int[rowCount];
            Array.Fill(nextValidPos, colCount - 1);
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = colCount - 1; j >= 0; j--)
                {
                    if ((fixedBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        nextValidPos[i] = j - 1;
                    }
                    if ((rockBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        rockBitmap[i] -= (UInt128)1 << j;
                        rockBitmap[i] += (UInt128)1 << nextValidPos[i];
                        nextValidPos[i]--;
                    }
                }
            }
            return;
        }

        public int CalculateNorthLoad()
        {
            int load = 0;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if ((rockBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        load += rowCount - i;
                    }
                }
            }
            return load;    
        }

        public string[] ToStringList()
        {
            string[] strings = new string[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if ((rockBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        strings[i] += 'O';
                        continue;
                    }
                    if ((fixedBitmap[i] & ((UInt128)1 << j)) > 0)
                    {
                        strings[i] += '#';
                        continue;
                    }
                    strings[i] += '.';
                }                
            }
            return strings;
        }

        public UInt128 GetHashKey()
        {
            UInt128 key = 0;
            foreach (var i in rockBitmap)
            {
                key += i;
            }
            return key;
        }

        public void PrintDish()
        {
            foreach (string s in ToStringList())
            {
                Console.WriteLine(s);
            }
            Console.WriteLine();
        }
    }

    public override string SolvePart2(string[] input)
    {
        Dish dish = Dish.FromStringList(input);

        Dictionary<UInt128, int> cache = [];

        int totalCycles = 1_000_000_000;
        for (int i = 0; i < totalCycles; i++)
        {
            if (!cache.TryAdd(dish.GetHashKey(), i))
            {
                int cyclesSinceLast = i - cache[dish.GetHashKey()];
                int cyclesLeft = totalCycles - i;
                i = totalCycles - cyclesLeft % cyclesSinceLast;
            }
            dish.TiltNorth();
            dish.TiltWest();
            dish.TiltSouth();
            dish.TiltEast();
        }
        int ans = dish.CalculateNorthLoad();
        return ans.ToString();
    }
}