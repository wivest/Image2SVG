using System.CommandLine;

namespace Image2SVG.Application
{
    class Application
    {
        private const string IMAGES_DIRECTORY = "images";
        private const string SAVE_DIRECTORY = "results";

        public DirectoryInfo LoadFolder { get; protected set; } = new(IMAGES_DIRECTORY);
        public DirectoryInfo SaveFolder { get; protected set; } = new(SAVE_DIRECTORY);

        public FileInfo? LoadFile { get; protected set; }
        public int ShapesCount { get; protected set; }
        public float Scale { get; protected set; }
        public int Samples { get; protected set; }
        public int Mutations { get; protected set; }
        public int Generations { get; protected set; }

        private readonly bool firstInitialization = false;

        public Application()
        {
            if (!LoadFolder.Exists || !SaveFolder.Exists)
                firstInitialization = true;
            LoadFolder.Create();
            SaveFolder.Create();
        }

        public int InvokeRootCommand(string[] args)
        {
            if (args.Length == 0 && firstInitialization)
                return 1;

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

            var root = new RootCommand("Translate raster image into .svg alterantive.");
            root.AddArgument(fileArgument);
            root.AddOption(shapesCountOption);
            root.AddOption(scaleOption);
            root.AddOption(samplesOption);
            root.AddOption(mutationsOption);
            root.AddOption(generationsOption);
            root.SetHandler(
                AssignParameters,
                fileArgument,
                shapesCountOption,
                scaleOption,
                samplesOption,
                mutationsOption,
                generationsOption
            );
            return root.Invoke(args);
        }

        private void AssignParameters(
            FileInfo file,
            int count,
            float scale,
            int samples,
            int mutations,
            int generations
        )
        {
            LoadFile = file;
            ShapesCount = count;
            Scale = scale;
            Samples = samples;
            Mutations = mutations;
            Generations = generations;
        }
    }
}
