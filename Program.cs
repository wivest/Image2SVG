using System.Globalization;
using Image2SVG.Application;
using Image2SVG.Shapes;

Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

const string IMAGE_FOLDER = "images/";
const string OUTPUT_FILENAME = "result";

Directory.CreateDirectory(IMAGE_FOLDER);
Directory.CreateDirectory(IMAGE_FOLDER + Image<Rect>.GENERATED_FOLDER);

if (args.Length < 1)
{
    Console.WriteLine("No input file specified.");
    return 1;
}

string inputFilename = args[0];

int shapes = 100;
if (args.Length >= 2)
{
    bool parsed = int.TryParse(args[1], out shapes);
    if (!parsed)
    {
        Console.WriteLine("Shape count is not a number.");
        return 1;
    }
}

try
{
    var image = new Image<Rect>(IMAGE_FOLDER + inputFilename);
    image.Generate(shapes);
    image.SaveTo(IMAGE_FOLDER, OUTPUT_FILENAME);
}
catch (FileLoadException)
{
    Console.WriteLine("Wrong file name.");
    return 1;
}

return 0;
