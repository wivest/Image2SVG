using FFMpegCore.Pipes;
using SkiaSharp;

namespace Image2SVG.Model
{
    class Frame : IVideoFrame, IDisposable
    {
        public int Width => source.Width;
        public int Height => source.Height;
        public string Format => "bgra";

        private readonly SKBitmap source;

        public Frame(SKBitmap bitmap)
        {
            source = bitmap;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        public void Serialize(Stream pipe)
        {
            throw new NotImplementedException();
        }

        public Task SerializeAsync(Stream pipe, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
