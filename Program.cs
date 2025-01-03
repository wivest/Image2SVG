﻿using System.Globalization;
using Image2SVG;
using Image2SVG.Model;

var application = new Application();

if (application.InvokeRootCommand(args) != 0)
    return 1;
if (application.LoadFile == null)
    return 1;

var image = new Image(application.LoadFile, application.Scale);
image.Generate(
    application.ShapesCount,
    application.Samples,
    application.Mutations,
    application.Generations
);
image.SaveTo(
    application.SaveFolder,
    application.LoadFile.Name,
    application.Frames,
    application.BinaryLocation
);

return 0;
