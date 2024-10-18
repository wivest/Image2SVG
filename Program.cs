using System.Globalization;
using Image2SVG.Application;
using Image2SVG.Shapes;

Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

var application = new Application(args);
bool loaded = application.TryLoadFile(out FileInfo file);
if (!loaded)
{
    Console.WriteLine("File not found.");
    return 1;
}

var image = new Image<Rect>(file);
image.Generate(application.ShapesCount);
image.SaveTo(application.SaveFolder, application.Filename);

return 0;
