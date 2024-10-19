using System.Globalization;
using Image2SVG.Application;
using Image2SVG.Shapes;

Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

var application = new Application(args);

var image = new Image<Rect>(application.LoadFile);
image.Generate(application.ShapesCount);
image.SaveTo(application.SaveFolder, application.LoadFile.Name);

return 0;
