﻿using System.Globalization;
using Image2SVG.Application;
using Image2SVG.Shapes;

Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

if (args.Length < 1)
{
    Console.WriteLine("No input file specified.");
    return 1;
}

string imageFilename = args[0];

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
    var application = new Application(args);
    var image = new Image<Rect>(application.ImageFile);
    image.Generate(shapes);
    image.SaveTo(application.SaveFolder, imageFilename);
}
catch (FileNotFoundException)
{
    Console.WriteLine("File not found.");
    return 1;
}

return 0;
