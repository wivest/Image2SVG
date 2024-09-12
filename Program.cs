using Image2SVG.Application;
using Image2SVG.Shapes;

const string IMAGE_FOLDER = "images/";
const string OUTPUT_FILENAME = "result.png";

if (args.Length < 1)
{
    Console.WriteLine("No input file specified.");
    return 1;
}

string inputFilename = args[0];

var image = new Image<Rect>(IMAGE_FOLDER + inputFilename);
image.Generate(10);
image.SaveTo(IMAGE_FOLDER + OUTPUT_FILENAME);

return 0;
