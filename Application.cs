using System.CommandLine;
using System.Text;

namespace Image2SVG
{
    class Application
    {
        private const string IMAGES_DIRECTORY = "images";
        private const string SAVE_DIRECTORY = "results";
        private const string FFMPEG_FILE = "ffmpeg.txt";
        private FileInfo ExecFile = new(FFMPEG_FILE);

        public DirectoryInfo LoadFolder { get; protected set; } = new(IMAGES_DIRECTORY);
        public DirectoryInfo SaveFolder { get; protected set; } = new(SAVE_DIRECTORY);

        public FileInfo? LoadFile { get; protected set; }

        public int ShapesCount { get; protected set; }
        public float Scale { get; protected set; }
        public int Samples { get; protected set; }
        public int Mutations { get; protected set; }
        public int Generations { get; protected set; }

        public int Frames { get; protected set; }

        public int InvokeRootCommand(string[] args)
        {
            var initCommand = new Command("init", "Initialize image folders.");
            var locationArgument = new Argument<DirectoryInfo>(
                name: "location",
                description: "Location of binary FFMPEG."
            );
            initCommand.AddArgument(locationArgument);
            initCommand.SetHandler((location) => InitializeFolders(location), locationArgument);

            var fileArgument = new Argument<FileInfo>(
                name: "file",
                description: "Load specified file.",
                parse: result =>
                {
                    string filename = result.Tokens.Single().Value;
                    string path = Path.Combine(LoadFolder.FullName, filename);
                    var file = new FileInfo(path);

                    if (!file.Exists)
                        result.ErrorMessage = "File doesn't exist.";
                    return file;
                }
            );
            var shapesCountOption = new Option<int>(
                name: "--count",
                description: "Count of generated shapes.",
                getDefaultValue: () => 100
            );
            var scaleOption = new Option<float>(
                name: "--scale",
                description: "New scale of image.",
                getDefaultValue: () => 0.2f
            );
            var samplesOption = new Option<int>(
                name: "--samples",
                description: "Generated shape samples.",
                getDefaultValue: () => 50
            );
            var mutationsOption = new Option<int>(
                name: "--mutations",
                description: "Mutations per shape.",
                getDefaultValue: () => 10
            );
            var generationsOption = new Option<int>(
                name: "--generations",
                description: "Number of generations.",
                getDefaultValue: () => 50
            );
            var framesOption = new Option<int>(
                name: "--frames",
                description: "Frame rate in animation (0 is no animation).",
                getDefaultValue: () => 0
            );

            var root = new RootCommand("Translate raster image into .svg alterantive.");

            root.AddCommand(initCommand);

            root.AddArgument(fileArgument);

            root.AddOption(shapesCountOption);
            root.AddOption(scaleOption);
            root.AddOption(samplesOption);
            root.AddOption(mutationsOption);
            root.AddOption(generationsOption);

            root.AddOption(framesOption);

            root.SetHandler(
                AssignParameters,
                fileArgument,
                shapesCountOption,
                scaleOption,
                samplesOption,
                mutationsOption,
                generationsOption,
                framesOption
            );

            return root.Invoke(args);
        }

        private void InitializeFolders(DirectoryInfo binaryLocation)
        {
            LoadFolder.Create();
            SaveFolder.Create();

            ExecFile.Delete();
            using FileStream stream = ExecFile.Create();
            byte[] buffer = new UTF8Encoding(true).GetBytes(binaryLocation.ToString());
            stream.Write(buffer);

            Console.WriteLine("Image folders were initialized.");
        }

        private void AssignParameters(
            FileInfo file,
            int count,
            float scale,
            int samples,
            int mutations,
            int generations,
            int frames
        )
        {
            LoadFile = file;
            ShapesCount = count;
            Scale = scale;
            Samples = samples;
            Mutations = mutations;
            Generations = generations;
            Frames = frames;
        }
    }
}
