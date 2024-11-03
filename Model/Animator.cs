using FFMpegCore.Pipes;

namespace Image2SVG.Model
{
    class Animator
    {
        public int frameRate { get; set; }

        private readonly List<Frame> frames = new();

        public void SaveTo(DirectoryInfo directory, string filename)
        {
            var videoSource = new RawVideoPipeSource(frames);
        }
    }
}
