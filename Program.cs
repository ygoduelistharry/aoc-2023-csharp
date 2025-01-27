using System.Reflection;
using System.Diagnostics;

class Program()
{

    static int year;
    static int day;
    static int part;
    static readonly string[] validActions = ["solve", "submit"];
    static string inputsFolder = Directory.GetCurrentDirectory() + "\\inputs";
    static string leadingZero = "";
    static string fileName = "";
    static string filePath = "";
    static string inputData = "";

    static async Task Main(string[] args)
    {

        // Validate inputs
        if (args.Length != 4)
        {
            Console.Error.Write("Please return 4 argments in {year day action part} format!\nFor example:\n dotnet run 2023 1 solve 1");
            return;
        }

        string yearString = args[0];
        string dayString = args[1];
        string actionString = args[2];
        string partString = args[3];


        if (!int.TryParse(yearString, out int y))
        {
            Console.Error.Write($"First argument must be a valid year! '{yearString}' is not valid");
            return;
        }

        if (y < 2015)
        {
            Console.Error.Write("AoC did not exist before 2015!");
            return;
        }

        if (!int.TryParse(dayString, out int d))
        {
            Console.Error.Write($"Second argument must be a valid day! '{dayString}' is not valid");
            return;
        }

        if (d < 1 || d > 25)
        {
            Console.Error.Write("Please select a valid day in December (between 1 and 25)!");
            return;
        }

        DateTime chosenDate = new(y, 12, d);
        if (chosenDate > DateTime.UtcNow)
        {
            Console.Error.Write("This puzzle is not released yet!");
            return;
        }

        if (!validActions.Contains(actionString))
        {
            Console.Error.Write($"Third argument must be a valid action! (must be one of {validActions})");
            return;
        }

        if (!int.TryParse(partString, out int p))
        {
            Console.Error.Write($"AoC problems only have 2 parts! Please enter 1 or 2.");
            return;
        }

        if (p != 1 && p != 2)
        {
            Console.Error.Write("AoC problems only have 2 parts! Please enter 1 or 2.");
            return;
        }

        (year, day, part) = (y, d, p);
        if (day < 10) { leadingZero = "0"; }
        fileName = $"{year}_{leadingZero}{day}.txt";
        filePath = Path.Combine(inputsFolder, fileName);

        // Set up the HTTP client
        HttpClient aocClient = new();
        aocClient.BaseAddress = new Uri("https://adventofcode.com");
        aocClient.DefaultRequestHeaders.UserAgent.TryParseAdd("ygoduelistharry@gmail.com on GitHub");
        string? aocSessionToken = Environment.GetEnvironmentVariable("AOC_SESSION");
        aocClient.DefaultRequestHeaders.Add("Cookie", $"session={aocSessionToken}");


        if (actionString == "solve" || actionString == "submit")
        {
            string leadingZero = "";
            if (day < 10) { leadingZero = "0"; }
            Type? solutionType = Type.GetType($"AoC{year}_{leadingZero}{day}");

            if (solutionType is null)
            {
                Console.Error.Write($"Solution class for AoC {year} day {day} doesn't exist!");
                return;
            }
            else
            {
                var solution = Activator.CreateInstance(solutionType);
                try
                {
                    string input = await GetPuzzleInput(aocClient);
                    string[] inputSplit = input.Split("\n");
                    // Input recieved always ends in a new line so we can ignore it.
                    inputSplit = inputSplit[..(inputSplit.Length - 1)];
                    MethodInfo? solver = solutionType.GetMethod($"SolvePart{part}");
                    if (solver is not null)
                    {
                        Stopwatch stopwatch = new();
                        stopwatch.Start();
                        string? ans = (string?)solver.Invoke(solution, [inputSplit]);
                        stopwatch.Stop();
                        if (ans is null) { return; }
                        Console.WriteLine($"Results for {year} day {day} part {part}:");
                        Console.WriteLine($"    Answer: {ans}");
                        Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds}ms");
                    }
                    else
                    {
                        Console.WriteLine("Could not find the solution method...");
                        return;
                    }
                }
                catch (HttpRequestException err)
                {
                    Console.Error.Write($"Failed to retrieve input from AoC server. Server responded with:\n{err.Message}\nPlease check your AoC session token!");
                    return;
                }
            }
        }
        if (actionString == "submit")
        {
            Console.WriteLine("todo: automatically submit the answer to AoC...");
        }
    }

    static async Task<string> GetPuzzleInput(HttpClient aocClient)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Retrieving input from AoC...");
            inputData = await aocClient.GetStringAsync($"{year}/day/{day}/input");
            using (StreamWriter inputFile = new(filePath))
            {
                inputFile.Write(inputData);
            }
            Console.WriteLine(inputData);
            Console.WriteLine("Input downloaded from AoC!");
        }
        else
        {
            using (StreamReader inputFile = new(filePath))
            {
                inputData = inputFile.ReadToEnd();
            }
            // Console.WriteLine(inputData);
            Console.WriteLine("Input already downloaded! Data read from cache.");
        }
        return inputData;
    }
}

class AoCSolution
{

    public virtual string? SolvePart1(string[] input)
    {
        Console.WriteLine("Solution not implemented for part 1 yet!");
        return null;
    }

    public virtual string? SolvePart2(string[] input)
    {
        Console.WriteLine("Solution not implemented for part 2 yet!");
        return null;
    }
}