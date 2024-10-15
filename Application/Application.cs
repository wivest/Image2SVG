namespace Image2SVG.Application
{
    class Application
    {
        private const string IMAGES_DIRECTORY = "images";
        private const string SAVE_DIRECTORY = "result";

        public DirectoryInfo LoadFolder { get; protected set; }
        public DirectoryInfo SaveFolder { get; protected set; }
        public FileInfo ImageFile { get; protected set; }

        public Application(string[] args)
        {
            LoadFolder = new(IMAGES_DIRECTORY);
            LoadFolder.Create();
            SaveFolder = LoadFolder.CreateSubdirectory(SAVE_DIRECTORY);

            string filename = args[0];
            string path = Path.Combine(LoadFolder.FullName, filename);
            ImageFile = new(path);
        }
    }
}
