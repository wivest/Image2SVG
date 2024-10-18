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

        private string filename;

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
                name: "count",
                description: "Count of generated shapes.",
                getDefaultValue: () => 100
            );

            var root = new RootCommand("Translate raster image into .svg alterantive.");
            root.AddArgument(filenameArgument);
            root.AddOption(shapesCountOption);
            root.SetHandler(
                (file, count) =>
                {
                    filename = file;
                    ShapesCount = count;
                },
                filenameArgument,
                shapesCountOption
            );
            root.Invoke(args);
        }

        public bool TryLoadFile(out FileInfo file)
        {
            string path = Path.Combine(LoadFolder.FullName, filename);
            file = new FileInfo(path);
            return file.Exists;
        }
    }
}
