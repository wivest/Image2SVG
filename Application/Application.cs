namespace Image2SVG.Application
{
    class Application
    {
        private const string IMAGES_DIRECTORY = "images";
        private const string SAVE_DIRECTORY = "result";

        public readonly DirectoryInfo LoadFolder = new(IMAGES_DIRECTORY);
        public readonly DirectoryInfo SaveFolder;

        public Application(string[] args)
        {
            LoadFolder.Create();
            SaveFolder = LoadFolder.CreateSubdirectory(SAVE_DIRECTORY);
        }
    }
}
