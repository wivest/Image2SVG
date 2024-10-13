namespace Image2SVG.Application
{
    class Application
    {
        private const string IMAGES_DIRECTORY = "images";
        private const string SAVE_DIRECTORY = "result";

        private readonly DirectoryInfo loadFolder = new(IMAGES_DIRECTORY);
        private readonly DirectoryInfo saveFolder;

        public Application(string[] args)
        {
            loadFolder.Create();
            saveFolder = loadFolder.CreateSubdirectory(SAVE_DIRECTORY);
        }
    }
}
