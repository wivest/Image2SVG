using System.CommandLine;

namespace Image2SVG.Application
{
    class Application
    {
        private const string IMAGES_DIRECTORY = "images";
        private const string SAVE_DIRECTORY = "result";

        public DirectoryInfo LoadFolder { get; protected set; }
        public DirectoryInfo SaveFolder { get; protected set; }
        public int ShapesCount { get; protected set; }

        public string Filename;

        public Application(string[] args)
        {
            LoadFolder = new(IMAGES_DIRECTORY);
            LoadFolder.Create();
            SaveFolder = LoadFolder.CreateSubdirectory(SAVE_DIRECTORY);

            var filenameArgument = new Argument<string>(
                name: "file",
                description: "Load specified file."
            );
            var shapesCountOption = new Option<int>(
                name: "--count",
                description: "Count of generated shapes.",
                getDefaultValue: () => 100
            );

            var root = new RootCommand("Translate raster image into .svg alterantive.");
            root.AddArgument(filenameArgument);
            root.AddOption(shapesCountOption);
            root.SetHandler(
                (file, count) =>
                {
                    Filename = file;
                    ShapesCount = count;
                },
                filenameArgument,
                shapesCountOption
            );
            root.Invoke(args);
        }

        public bool TryLoadFile(out FileInfo file)
        {
            string path = Path.Combine(LoadFolder.FullName, Filename);
            file = new FileInfo(path);
            return file.Exists;
        }
    }
}
