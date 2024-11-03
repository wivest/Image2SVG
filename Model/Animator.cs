using FFMpegCore.Pipes;
using SkiaSharp;

namespace Image2SVG.Model
{
    class Animator
    {
        public int FrameRate { get; set; }

        private readonly List<Frame> frames = new();

        public void SaveTo(DirectoryInfo directory, string filename)
        {
            var videoSource = new RawVideoPipeSource(frames) { FrameRate = FrameRate };
        }

        public void AddFrame(SKBitmap bitmap)
        {
            frames.Add(new Frame(bitmap));
        }
    }
}
