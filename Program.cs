// See https://aka.ms/new-console-template for more information

using Microsoft.ML;
using Microsoft.ML.Vision;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using Microsoft.ML.Transforms.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;


Console.WriteLine("Cutting and Drafting Images trian them ");


         string inputImagePath = "//Users//syedqadri//Documents//Dev//GPTCODEIDENTIFYANDTRAINIMAGES/imagerepository/engasl.png";
        string outputFolderPath = "//Users/syedqadri/Documents/Dev/GPTCODEIDENTIFYANDTRAINIMAGES/imagerepository/";
        // assuming cols or counted from image but this also needs to be caluclated by logic
        int rows = 4;
        int cols = 7;

        var croppedImagePaths = Cutter.SplitImage(inputImagePath, outputFolderPath, rows, cols);

      var mlContext = new MLContext();
        var imageData = new List<ImageData>();

        foreach (var path in croppedImagePaths)
        {
            imageData.Add(new ImageData { Image = path, Label = "Unknown" });
        }

        var dataView = mlContext.Data.LoadFromEnumerable(imageData);

        Console.WriteLine("Data loaded into IDataView successfully.");

       

        var matrix = Cutter.LoadMatrixFromFolder(outputFolderPath, rows, cols);



        foreach (var box in matrix.Boxes)
        {
           var lbl = Cutter.GetTextFromImageIron(box.ImagePath);
            imageData.Add(new ImageData { Image = box.ImagePath, Label = lbl });
        }


        var data = mlContext.Data.LoadFromEnumerable(imageData);
var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label")
    .Append(mlContext.Transforms.LoadImages(
        outputColumnName: "Image",
        imageFolder: "",
        inputColumnName: "Image"))
    .Append(mlContext.Transforms.ResizeImages(
        outputColumnName: "Image", imageWidth: 224, imageHeight: 224))
    .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "Image"))
    // Custom model loading here (optional)
    //.Append(mlContext.Model.LoadTensorFlowModel("model.pb").AddInput("input", "Image").AddOutput("output"))
    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "Label"));

        var model = pipeline.Fit(data);

        mlContext.Model.Save(model, data.Schema, "image_box_model.tar.zip");

        Console.WriteLine("Model trained and saved.");

        /*
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
Looks like you're running into a common issue when working with **System.Drawing.Common** on **.NET 8** on **macOS**. Starting from **.NET 6**, **System.Drawing.Common** is no longer fully supported on non-Windows platforms. This is likely the reason you're getting that `gdiplus.dll` error.

### Why It’s Happening:
- **System.Drawing.Common** depends on **GDI+**, which is a Windows graphics library. It’s not natively available on macOS.
- You are likely trying to manipulate images (cutting, drafting, or drawing) using this library, but it requires **gdiplus.dll**, which macOS doesn’t have.

### Solutions to Fix It:

#### 1. **Switch to a Cross-Platform Image Library:**
The best approach is to move away from **System.Drawing.Common**. You can use these libraries instead:
- **SkiaSharp** → Great for 2D graphics and image manipulation.
- **ImageSharp** → Pure .NET, cross-platform image processing library.

**Example with ImageSharp:**
```csharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

using (var image = Image.Load("image.png"))
{
    image.Mutate(x => x.Resize(200, 200));
    image.Save("resized_image.png");
}
```

#### 2. **If You Really Need System.Drawing.Common** (Not Recommended):
- Install **libgdiplus** on macOS:
  ```bash
  brew install mono-libgdiplus
  ```

- Then set the compatibility flag:
  ```bash
  DOTNET_SYSTEM_DRAWING_ENABLE_MACOS_SUPPORT=1
  ```

- Or, in your code (for **.NET 6+**):
  ```csharp
  AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);
  ```

But honestly, this is a bit fragile and not future-proof. Microsoft recommends not using **System.Drawing.Common** on non-Windows platforms anymore.

### Recommended Approach for Your Project:
Since you're working on image processing for your **image recognition project**, I'd suggest using **ImageSharp**. It’s fast, modern, and works across Windows, Linux, and macOS.

Would you like help converting your image cutting and drafting logic to **ImageSharp** or **SkiaSharp**?

using (var image = Image.Load("image.png"))
{
    image.Mutate(x => x.Resize(200, 200));
    image.Save("resized_image.png");
}


Notes for Tensor Flow 
1. Download the Precompiled TensorFlow C Library:
Go to: TensorFlow C Library Releases
Find the latest release (e.g., libtensorflow-macos-x86_64.tar.gz or libtensorflow-macos-arm64.tar.gz depending on your chip: Intel or Apple Silicon).
Download the appropriate version based on your architecture:
Apple Silicon (M1, M2) → macos-arm64
Intel Mac → macos-x86_64
2. Extract and Install:
sudo mkdir -p /usr/local/lib
sudo mkdir -p /usr/local/include/tensorflow
Extract the downloaded .tar.gz file:

tar -xvzf libtensorflow-macos-<arch>.tar.gz
// check your installations
ls -l /usr/local/lib | grep libtensorflow


sudo codesign --force --deep --sign - /usr/local/lib/libtensorflow.2.18.0.dylib
sudo codesign --force --deep --sign - /usr/local/lib/libtensorflow_framework.2.18.0.dylib

        */