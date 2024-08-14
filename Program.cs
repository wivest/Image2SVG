using SkiaSharp;

if (args.Length == 0)
{
    Console.WriteLine("No file specified.");
    return 1;
}

string filename = args[0];
SKImage image = SKImage.FromEncodedData(filename);
if (image == null)
{
    Console.WriteLine("Invalid file.");
    return 1;
}

return 0;
