class AoC2023_22 : AoCSolution
{
    class Brick : IComparable<Brick>
    {
        (int x, int y, int z) pointA;
        (int x, int y, int z) pointB;
        public int height;
        public List<(int x, int y)> footPrint = [];

        public int baseZ;
        public HashSet<Brick> supportedBy = [];
        public HashSet<Brick> supports = [];

        void SetFootprint()
        {
            for (int i = 0; i <= pointB.x - pointA.x; i++)
            {
                for (int j = 0; j <= pointB.y - pointA.y; j++)
                {
                    footPrint.Add((pointA.x + i, pointA.y + j));
                }
            }
            height = pointB.z - pointA.z + 1;
            baseZ = pointA.z;
            return;
        }

        public static Brick FromString(string s)
        {
            string[] vals = s.Split([',', '~']);
            Brick brick = new()
            {
                pointA = (int.Parse(vals[0]), int.Parse(vals[1]), int.Parse(vals[2])),
                pointB = (int.Parse(vals[3]), int.Parse(vals[4]), int.Parse(vals[5])),
            };
            brick.SetFootprint();
            return brick;
        }

        // the brick-dropping algorithm requires the bricks to be sorted by z position
        public int CompareTo(Brick? other)
        {
            if (other is null) { return 1; }
            return baseZ - other.baseZ;
        }

        public Brick Clone()
        {
            Brick newBrick = new()
            {
                baseZ = this.baseZ,
                height = this.height,
                pointA = this.pointA,
                pointB = this.pointB,
                footPrint = [.. this.footPrint]
            };
            return newBrick;
        }
    }


    static List<Brick> ProcessInputs(string[] input)
    {
        return [.. input.Select(Brick.FromString)];
    }


    static int DropBricks(List<Brick> brickData)
    {
        brickData.Sort();

        Dictionary<(int x, int y, int z), int> finalCubePositions = [];
        int[,] heightMap = new int[10, 10];
        // count and return the number of fallen bricks for part 2
        int fallenBricksCount = 0;

        for (int i = 0; i < brickData.Count; i++)
        {
            var brick = brickData[i];
            int supportBlockZ = 0;
            foreach (var (x, y) in brick.footPrint)
            {
                supportBlockZ = Math.Max(supportBlockZ, heightMap[x, y]);
            }
            if (brick.baseZ != supportBlockZ + 1)
            {
                brick.baseZ = supportBlockZ + 1;
                fallenBricksCount++;
            }
            foreach (var (x, y) in brick.footPrint)
            {
                for (int z = 0; z < brick.height; z++)
                {
                    finalCubePositions.Add((x, y, brick.baseZ + z), i);
                }
                if (finalCubePositions.TryGetValue((x, y, supportBlockZ), out int id))
                {
                    brick.supportedBy.Add(brickData[id]);
                    brickData[id].supports.Add(brick);
                }
                heightMap[x, y] = supportBlockZ + brick.height;
            }
        }
        return fallenBricksCount;
    }


    static HashSet<Brick> FindEssentialBricks(List<Brick> brickData)
    {
        HashSet<Brick> essentialBricks = [];
        foreach (var brick in brickData)
        {
            if (brick.supportedBy.Count == 1)
            {
                essentialBricks.Add(brick.supportedBy.First());
            }
        }
        return essentialBricks;
    }

    public override string SolvePart1(string[] input)
    {
        List<Brick> allBricks = ProcessInputs(input);
        DropBricks(allBricks);
        HashSet<Brick> essentialBricks = FindEssentialBricks(allBricks);
        int ans = allBricks.Count - essentialBricks.Count;
        return ans.ToString();
    }

    public override string SolvePart2(string[] input)
    {
        int ans = 0;
        List<Brick> allBricks = ProcessInputs(input);
        DropBricks(allBricks);
        HashSet<Brick> essentialBricks = FindEssentialBricks(allBricks);

        // i'm sure theres a smarter way to do this
        // but i just drop all the bricks (same as part 1)
        // then remove one essential brick and drop them again
        // count how many fall and add to answer
        // reset and repeat for every essential brick

        // part 1 takes <10ms on my machine
        // part 2 takes <1000ms i was expecting worse!
        // but i think parsing the input string is the slow bit
        foreach (var removedBrick in essentialBricks)
        {
            List<Brick> almostAllBricks = [];
            foreach (var brick in allBricks)
            {
                if (brick != removedBrick) { almostAllBricks.Add(brick.Clone()); }
            }
            ans += DropBricks(almostAllBricks);
        }
        return ans.ToString();
    }
}