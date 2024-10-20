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
                if (!result.GetValueForArgument(fileArgument).Exists)
                    result.ErrorMessage = "File doesn't exist.";
            });
            var shapesCountOption = new Option<int>(
                name: "--count",
                description: "Count of generated shapes.",
                getDefaultValue: () => 100
            );

            var root = new RootCommand("Translate raster image into .svg alterantive.");
            root.AddArgument(fileArgument);
            root.AddOption(shapesCountOption);
            root.SetHandler(AssignParameters, fileArgument, shapesCountOption);
            return root.Invoke(args);
        }

        private void AssignParameters(FileInfo file, int count)
        {
            string path = Path.Combine(LoadFolder.FullName, file.Name);
            LoadFile = new FileInfo(path);
            ShapesCount = count;
        }
    }
}
