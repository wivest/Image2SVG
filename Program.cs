using Image2SVG.Application;
using Image2SVG.Shapes;
using SkiaSharp;

if (args.Length < 2)
{
    Console.WriteLine("No input AND output file specified.");
    return 1;
}

string inputFilename = $"images/{args[0]}";
string outputFilename = $"images/{args[1]}";

var image = new Image(inputFilename);
image.Generate<Rect>(100);
image.SaveTo(outputFilename);

return 0;
