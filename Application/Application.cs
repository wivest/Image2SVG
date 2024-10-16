namespace Image2SVG.Application
{
    class Application
    {
        private const string IMAGES_DIRECTORY = "images";
        private const string SAVE_DIRECTORY = "result";

        public DirectoryInfo LoadFolder { get; protected set; }
        public DirectoryInfo SaveFolder { get; protected set; }

        private string filename;

        public Application(string[] args)
        {
            LoadFolder = new(IMAGES_DIRECTORY);
            LoadFolder.Create();
            SaveFolder = LoadFolder.CreateSubdirectory(SAVE_DIRECTORY);

            filename = args[0];
        }

        public bool TryLoadFile(out FileInfo file)
        {
            string path = Path.Combine(LoadFolder.FullName, filename);
            file = new FileInfo(path);
            return file.Exists;
        }
    }
}
