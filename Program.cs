using Image2SVG.Image;
using SkiaSharp;

if (args.Length == 0)
{
    Console.WriteLine("No file specified.");
    return 1;
}

string filename = args[0];
SKImage skImage = SKImage.FromEncodedData(filename);
if (skImage == null)
{
    Console.WriteLine("Invalid file.");
    return 1;
}

var image = new Image(skImage);

return 0;
