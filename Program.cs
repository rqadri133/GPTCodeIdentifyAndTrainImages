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
using Microsoft.ML.Transforms.Onnx;



Console.WriteLine("Cutting and Drafting Images trian them ");


         string inputImagePath = "//Users//syedqadri//Documents//Dev//GPTCODEIDENTIFYANDTRAINIMAGES/imagerepository/aslsample.png";
        string outputFolderPath = "//Users/syedqadri/Documents/Dev/GPTCODEIDENTIFYANDTRAINIMAGES/imagerepository/";
        // assuming cols or counted from image but this also needs to be caluclated by logic
        int rows = 5;
        int cols = 5;
        string onnxModelPath = Path.Combine(Directory.GetCurrentDirectory(), "imagerepository/mobilenetv2.onnx");

        var croppedImagePaths = Cutter.SplitImage(inputImagePath, outputFolderPath, rows, cols);

      var mlContext = new MLContext();
        var imageData = new List<ImageData>();

        foreach (var dictval in croppedImagePaths)
        {
            imageData.Add(new ImageData { Image = dictval.Value.imagePath, Label = dictval.Value.Alphabet.ToString() });
        }

        var dataView = mlContext.Data.LoadFromEnumerable(imageData);



        var onnxOptions = new OnnxOptions()
        {
            ModelFile = onnxModelPath, // Correct way to specify the ONNX model
            InputColumns = new[] { "Image" },
            OutputColumns = new[] { "Features" }
        };

        // Define ASL pipeline
       var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
            .Append(mlContext.Transforms.LoadImages("Image", outputFolderPath, nameof(ImageData.Image)))
            .Append(mlContext.Transforms.ResizeImages("Image", 224, 224))
            .Append(mlContext.Transforms.ExtractPixels("Image"))
            .Append(mlContext.Transforms.ApplyOnnxModel(onnxOptions)) // Fixed ApplyOnnxModel
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        // Train and save the model
        var model = pipeline.Fit(dataView);
        mlContext.Model.Save(model, dataView.Schema, outputFolderPath);
        Console.WriteLine("ASL Model trained and saved successfully!");
     //   var testData = new ImageData() { Image = outputFolderPath + "//box_2_0.png" };

Cutter.TestModel(mlContext, outputFolderPath ,  outputFolderPath + "//box_2_0.png");
             
       
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
