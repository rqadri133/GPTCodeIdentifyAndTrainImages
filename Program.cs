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


         string inputImagePath = "//Users//syedqadri//Documents//Dev//GPTCODEIDENTIFYANDTRAINIMAGES/imagerepository/aslsample.png";
        string outputFolderPath = "//Users/syedqadri/Documents/Dev/GPTCODEIDENTIFYANDTRAINIMAGES/imagerepository/";
        // assuming cols or counted from image but this also needs to be caluclated by logic
        int rows = 5;
        int cols = 5;

        var croppedImagePaths = Cutter.SplitImage(inputImagePath, outputFolderPath, rows, cols);

      var mlContext = new MLContext();
        var imageData = new List<ImageData>();

        foreach (var dictval in croppedImagePaths)
        {
            imageData.Add(new ImageData { Image = dictval.Value.imagePath, Label = dictval.Value.Alphabet.ToString() });
        }

        var dataView = mlContext.Data.LoadFromEnumerable(imageData);

        Console.WriteLine("Data loaded into IDataView successfully.");

       
     
     
       /* var data = mlContext.Data.LoadFromEnumerable(imageData);
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
       DataViewSchema dataPrepPipelineSchema, modelSchema;

  ITransformer dataPrepPipeline = mlContext.Model.Load("image_box_model.tar.zip",out dataPrepPipelineSchema);
   ITransformer trainedModel = mlContext.Model.Load("image_box_model.tar.zip", out modelSchema);
trainedModel.Preview(dataView,200); */

var imageDataView = mlContext.Data.LoadFromEnumerable(imageData);

var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label")
    .Append(mlContext.Transforms.LoadImages(
        outputColumnName: "Image",
        imageFolder: "imagerepository",  // <-- Set correct path
        inputColumnName: "Image"))
    .Append(mlContext.Transforms.ResizeImages(
        outputColumnName: "Image", imageWidth: 224, imageHeight: 224))
    .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "Image"))
    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "Label"));

var model = pipeline.Fit(imageDataView);

// Save the model
mlContext.Model.Save(model, imageDataView.Schema, "image_box_model.tar.zip");

// Load the model for inference
DataViewSchema modelSchema;
ITransformer trainedModel = mlContext.Model.Load("image_box_model.tar.zip", out modelSchema);

// Load test data (ensure this is properly set)
var testData = mlContext.Data.LoadFromEnumerable(imageData); 
var predictions = trainedModel.Transform(testData);

// Preview the results
var preview = predictions.Preview(200);

        Console.WriteLine("Model trained and saved.");

/// <summary>
/// Test it
/// </summary>
 string modelPath = "image_box_model.tar.zip";
        string testImagePath = "//Users/syedqadri/Documents/Dev/GPTCODEIDENTIFYANDTRAINIMAGES/imagerepository/box_2_0.png"; // Change this to your test image path

        var mlContextTest = new MLContext();

        // Load the trained model
        ITransformer trainedModelM = mlContext.Model.Load(modelPath, out var modelSchemaM);

        // Define image input schema
        var data = new ImageData { Image = testImagePath };
        var imageDataViewM = mlContext.Data.LoadFromEnumerable(new[] { data });

        // Transform image data using the trained model
        IDataView transformedData = trainedModel.Transform(imageDataViewM);

        // Extract and display the prediction
      ITransformer trainedModelMi = mlContext.Model.Load(modelPath, out var modelSchemaMi);

        // Print model output schema
        Console.WriteLine("Model Output Schema:");
        foreach (var column in modelSchemaMi)
        {
            Console.WriteLine($"Column Name: {column.Name}, Type: {column.Type}");
        }

        var predictionResults = mlContext.Data.CreateEnumerable<PredictionResult>(predictions, reuseRowObject: false);

        foreach (var prediction in predictionResults)
        {
            Console.WriteLine($"Predicted Label: {prediction.Label}");
            Console.WriteLine($"Confidence Scores: {string.Join(", ", prediction.PredictedLabel)}");
        }
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

        public class TestImage
    {
        [LoadColumn(0)]
        public string ImagePath { get; set; }

     [LoadColumn(1)]
        public string Label { get; set; }
        
    
    }

   public class PredictionResult
{
    [ColumnName("PredictedLabel")]
    public string PredictedLabel { get; set; }

    // For classification, if the model outputs a label as Key<UInt32>
    [ColumnName("Label")]
    public uint Label { get; set; }  // For numerical (or categorical) labels
}
