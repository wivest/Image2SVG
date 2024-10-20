﻿using System.Globalization;
using Image2SVG.Application;
using Image2SVG.Shapes;

Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

var application = new Application();

if (application.InvokeRootCommand(args) != 0)
    return 1;
if (application.LoadFile == null)
    return 1;

var image = new Image<Rect>(application.LoadFile);
image.Generate(application.ShapesCount);
image.SaveTo(application.SaveFolder, application.LoadFile.Name);

return 0;
