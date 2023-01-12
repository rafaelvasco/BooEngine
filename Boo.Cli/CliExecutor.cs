using Boo.Cli.Builders;
using Boo.Common.Content;
using PowerArgs;

namespace Boo.Cli;

public struct BuildActionArgs
{
    [ArgRequired(PromptIfMissing = true), ArgDescription("Game Folder"), ArgPosition(1), ArgShortcut("-g")]
    public string GameFolder { get; set; }
}

[ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
public class CliExecutor
{
    [HelpHook, ArgShortcut("-?"), ArgDescription("Shows Usage Options")]
    public bool Help { get; set; }

    [ArgActionMethod, ArgDescription("Builds Game Assets"), ArgShortcut("b")]
    public static void Build(BuildActionArgs args)
    {

        var gameFolderArg = args.GameFolder;

        try
        {
            var assetsFullPath = Path.Combine(gameFolderArg, ContentProperties.AssetsFolder);

            if (!Directory.Exists(assetsFullPath))
            {
                throw new ApplicationException("Could not find Assets folder");
            }

            AssetBuilder.BuildAssets(assetsFullPath);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}