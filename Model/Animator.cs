using FFMpegCore;
using FFMpegCore.Pipes;
using SkiaSharp;

namespace Image2SVG.Model
{
    class Animator
    {
        private const string FFMPEG_PATH = @"";

        public int FrameRate { get; set; }

        private readonly List<Frame> frames = new();

        public Animator()
        {
            string? path = Environment.GetEnvironmentVariable("PATH");
            Environment.SetEnvironmentVariable("PATH", $"{path};{FFMPEG_PATH}");
        }

        public bool SaveTo(DirectoryInfo directory, string filename)
        {
            string path = $"{directory.FullName}/{filename}.webm";

            var videoSource = new RawVideoPipeSource(frames) { FrameRate = FrameRate };
            return FFMpegArguments
                .FromPipeInput(videoSource)
                .OutputToFile(path, addArguments: options => options.WithVideoCodec("libvpx-vp9"))
                .ProcessSynchronously();
        }

        public void AddFrame(SKBitmap bitmap)
        {
            frames.Add(new Frame(bitmap));
        }
    }
}
