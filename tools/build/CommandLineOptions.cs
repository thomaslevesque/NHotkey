using static Bullseye.Targets;

internal record CommandLineOptions(string Configuration, bool ShowHelp, string[] BullseyeArgs)
{
    public static CommandLineOptions Parse(string[] args)
    {
        var bullseyeArgs = new List<string>();
        string configuration = "Release";
        bool showHelp = false;
        using var enumerator = ((IEnumerable<string>)args).GetEnumerator();
        while (enumerator.MoveNext())
        {
            var arg = enumerator.Current;
            if (arg is "-h" or "--help")
            {
                showHelp = true;
                break;
            }
            else if (arg is "-c" or "--configuration")
            {
                configuration = ReadOptionValue(arg);
            }
            else
            {
                bullseyeArgs.Add(arg);
            }
        }

        return new(configuration, showHelp, bullseyeArgs.ToArray());

        string ReadOptionValue(string arg)
        {
            if (!enumerator.MoveNext())
                throw new InvalidOperationException($"Expected value for option '{arg}', but none was found.");

            return enumerator.Current;
        }
    }

    public static async Task PrintUsageAsync()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  build [-c|--configuration <buildConfiguration>] <bullseyeArgs>");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  <bullseyeArguments>  Arguments to pass to Bullseye (targets and options, see below)");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -c, --configuration <buildConfiguration>  The configuration to build [default: Release]");
        Console.WriteLine("  -? -h, --help                             Show help and usage information");
        Console.WriteLine();
        Console.WriteLine("Bullseye help:");
        await RunTargetsWithoutExitingAsync(["--help"]);
    }
}