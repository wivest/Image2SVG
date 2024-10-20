using System.CommandLine;

namespace Image2SVG.Application
{
    class Application
    {
        private const string IMAGES_DIRECTORY = "images";
        private const string SAVE_DIRECTORY = "result";

        public DirectoryInfo LoadFolder { get; protected set; }
        public DirectoryInfo SaveFolder { get; protected set; }

        public FileInfo? LoadFile { get; protected set; }
        public int ShapesCount { get; protected set; }
        public float Scale { get; protected set; }
        public int Samples { get; protected set; }
        public int Mutations { get; protected set; }
        public int Generations { get; protected set; }

        public Application()
        {
            LoadFolder = new(IMAGES_DIRECTORY);
            LoadFolder.Create();
            SaveFolder = LoadFolder.CreateSubdirectory(SAVE_DIRECTORY);
        }

        public int InvokeRootCommand(string[] args)
        {
            var fileArgument = new Argument<FileInfo>(
                name: "file",
                description: "Load specified file."
            );
            fileArgument.AddValidator(result =>
            {
                FileInfo single = result.GetValueForArgument(fileArgument);
                var file = new FileInfo(GetFilePath(single.Name));
                if (!file.Exists)
                    result.ErrorMessage = "File doesn't exist.";
            });
            var shapesCountOption = new Option<int>(
                name: "--count",
                description: "Count of generated shapes.",
                getDefaultValue: () => 100
            );
            var scaleOption = new Option<float>(
                name: "--scale",
                description: "Count of generated shapes.",
                getDefaultValue: () => 0.2f
            );
            var samplesOption = new Option<int>(
                name: "--samples",
                description: "Count of generated shapes.",
                getDefaultValue: () => 50
            );
            var mutationsOption = new Option<int>(
                name: "--mutations",
                description: "Count of generated shapes.",
                getDefaultValue: () => 10
            );
            var generationsOption = new Option<int>(
                name: "--generations",
                description: "Count of generated shapes.",
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
            LoadFile = new FileInfo(GetFilePath(file.Name));
            ShapesCount = count;
            Scale = scale;
            Samples = samples;
            Mutations = mutations;
            Generations = generations;
        }

        private string GetFilePath(string filename)
        {
            return Path.Combine(LoadFolder.FullName, filename);
        }
    }
}
