﻿using Image2SVG.Image;
using SkiaSharp;

if (args.Length < 2)
{
    Console.WriteLine("No input AND output file specified.");
    return 1;
}

string inputFilename = args[0];
string outputFilename = args[1];

SKImage skImage = SKImage.FromEncodedData(inputFilename);
if (skImage == null)
{
    Console.WriteLine("Invalid file.");
    return 1;
}

var image = new Image(skImage);

return 0;
