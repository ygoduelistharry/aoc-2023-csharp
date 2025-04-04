using System.Text.RegularExpressions;

class AoC2023_24 : AoCSolution
{
    record struct Vector(long x, long y, long z, int vx, int vy, int vz);

    List<Vector> ProcessInputs(string[] input)
    {
        List<Vector> vectors = [];
        foreach (string s in input)
        {
            var numbers = Regex.Matches(s, @"[\d-]+");
            long x = long.Parse(numbers[0].ToString());
            long y = long.Parse(numbers[1].ToString());
            long z = long.Parse(numbers[2].ToString());
            int vx = int.Parse(numbers[3].ToString());
            int vy = int.Parse(numbers[4].ToString());
            int vz = int.Parse(numbers[5].ToString());
            vectors.Add(new(x, y, z, vx, vy, vz));
        }
        return vectors;
    }

    static bool Find2DIntersect(Vector vec1, Vector vec2, out (float x, float y) intersect)
    {
        var (x1, y1, _, vx1, vy1, _) = vec1;
        var (x2, y2, _, vx2, vy2, _) = vec2;

        long det = vx2 * vy1 - vx1 * vy2;
        if (det == 0) { intersect = default; return false; }

        float s = 1 / (float)det * (-vy2 * (x2 - x1) + vx2 * (y2 - y1));
        float t = 1 / (float)det * (-vy1 * (x2 - x1) + vx1 * (y2 - y1));

        if (s <= 0 || t <= 0) { intersect = default; return false; }

        intersect = (x1 + vx1 * s, y1 + vy1 * s);
        return true;
    }

    public override string SolvePart1(string[] input)
    {
        int ans = 0;
        var vectors = ProcessInputs(input);
        for (int i = 0; i < vectors.Count; i++)
        {
            for (int j = 0; j < vectors.Count; j++)
            {
                if (j <= i) { continue; }
                if (Find2DIntersect(vectors[i], vectors[j], out var intersect))
                {
                    if (intersect.x >= 200000000000000 &&
                        intersect.x <= 400000000000000 &&
                        intersect.y >= 200000000000000 &&
                        intersect.y <= 400000000000000)
                    {
                        ans += 1;
                    }
                }
            }
        }
        return ans.ToString();
    }

    static decimal[] GaussianElimination(decimal[][] Ay)
    {
        int eqCount = Ay.Length;

        for (int eq = 0; eq < eqCount; eq++)
        {
            int pivRow = eq;
            int pivCol = eq;
            //find the max in the piv col
            decimal pivVal = Ay[pivRow][pivCol];
            int maxRow = pivRow;
            for (int r = pivRow + 1; r < eqCount; r++)
            {
                if (Math.Abs(Ay[r][pivCol]) > Math.Abs(pivVal))
                {
                    pivVal = Ay[r][pivCol];
                    maxRow = r;
                }
            }
            // swap the values in the max row to the pivot row
            if (maxRow != pivRow)
            {
                (Ay[maxRow], Ay[pivRow]) = (Ay[pivRow], Ay[maxRow]);
            }
            // divide the values in the pivot row by the pivot value
            // everything to the left of the pivot column should be 0
            Ay[pivRow][pivCol] = 1;
            for (int c = pivCol + 1; c <= eqCount; c++)
            {
                Ay[pivRow][c] /= pivVal;
            }

            // iterate over all the elements and do the right thing
            for (int r = 0; r < eqCount; r++)
            {
                decimal scale = Ay[r][pivCol];
                for (int c = pivCol; c <= eqCount; c++)
                {
                    if (r == pivRow) { continue; }
                    if (c == pivCol) { Ay[r][c] = 0; continue; }
                    Ay[r][c] -= scale * Ay[pivRow][c];
                }
            }
        }
        decimal[] ans = new decimal[eqCount];
        for (int x = 0; x < eqCount; x++)
        {
            ans[x] = Math.Round(Ay[x][^1], 0);
        }
        return ans;
    }

    public override string SolvePart2(string[] input)
    {
        var vectors = ProcessInputs(input);
        // just did a bunch of maths and solve 6 simultaneous linear eqs

        // we just need 3 points for this:
        var (x0, y0, z0, vx0, vy0, vz0) = vectors[0];
        var (x1, y1, z1, vx1, vy1, vz1) = vectors[1];
        var (x2, y2, z2, vx2, vy2, vz2) = vectors[2];

        // relevant coefficients
        long cx1 = vy0 - vy1; long cy1 = vx1 - vx0; long cvx1 = y1 - y0; long cvy1 = x0 - x1;
        long cx2 = vz0 - vz1; long cz2 = vx1 - vx0; long cvx2 = z1 - z0; long cvz2 = x0 - x1;
        long cy3 = vz0 - vz1; long cz3 = vy1 - vy0; long cvy3 = z1 - z0; long cvz3 = y0 - y1;
        long rhs1 = x0 * vy0 - vx0 * y0 - x1 * vy1 + vx1 * y1;
        long rhs2 = x0 * vz0 - vx0 * z0 - x1 * vz1 + vx1 * z1;
        long rhs3 = y0 * vz0 - vy0 * z0 - y1 * vz1 + vy1 * z1;

        long cx4 = vy0 - vy2; long cy4 = vx2 - vx0; long cvx4 = y2 - y0; long cvy4 = x0 - x2;
        long cx5 = vz0 - vz2; long cz5 = vx2 - vx0; long cvx5 = z2 - z0; long cvz5 = x0 - x2;
        long cy6 = vz0 - vz2; long cz6 = vy2 - vy0; long cvy6 = z2 - z0; long cvz6 = y0 - y2;
        long rhs4 = x0 * vy0 - vx0 * y0 - x2 * vy2 + vx2 * y2;
        long rhs5 = x0 * vz0 - vx0 * z0 - x2 * vz2 + vx2 * z2;
        long rhs6 = y0 * vz0 - vy0 * z0 - y2 * vz2 + vy2 * z2;

        decimal[] eq1 = [cx1, cy1, 0, cvx1, cvy1, 0, rhs1];
        decimal[] eq2 = [cx2, 0, cz2, cvx2, 0, cvz2, rhs2];
        decimal[] eq3 = [0, cy3, cz3, 0, cvy3, cvz3, rhs3];
        decimal[] eq4 = [cx4, cy4, 0, cvx4, cvy4, 0, rhs4];
        decimal[] eq5 = [cx5, 0, cz5, cvx5, 0, cvz5, rhs5];
        decimal[] eq6 = [0, cy6, cz6, 0, cvy6, cvz6, rhs6];

        decimal[][] eqs = [eq1, eq2, eq3, eq4, eq5, eq6];

        decimal[] sol = GaussianElimination(eqs);

        decimal ans = sol[0] + sol[1] + sol[2];
        return ans.ToString();
    }
}