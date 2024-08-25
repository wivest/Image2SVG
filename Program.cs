using Image2SVG.Image;
using Image2SVG.Shapes;
using SkiaSharp;

if (args.Length < 2)
{
    Console.WriteLine("No input AND output file specified.");
    return 1;
}

string inputFilename = $"images/{args[0]}";
string outputFilename = $"images/{args[1]}";

SKImage skImage = SKImage.FromEncodedData(inputFilename);
if (skImage == null)
{
    Console.WriteLine("Invalid file.");
    return 1;
}

var image = new Image(skImage, SKColors.Red);
image.Generate(new Rect { Alpha = 128 }, 100, 10);
image.SaveTo(outputFilename);

return 0;
