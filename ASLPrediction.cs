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
using OpenCvSharp;
using OpenTK.Graphics.OpenGL;

public class ASLRecognition
{
    static readonly string dataPath = "./asl_images"; // Folder with images
    static readonly string modelPath = "asl_model.zip";
    static MLContext mlContext = new MLContext();
    private NvVideoCapture capture;
    private Mat frame;

    public ASLRecognition()
    {
        TrainModel();
        StartCamera();
    }

    private void TrainModel()
    {
        var images = Directory.EnumerateFiles(dataPath, "*.png", SearchOption.AllDirectories)
            .Select(path => new ImageData { Image = path, Label = Path.GetFileNameWithoutExtension(path) })
            .ToList();

        var data = mlContext.Data.LoadFromEnumerable(images);
        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label")
            .Append(mlContext.Transforms.LoadRawImageBytes("ImagePath", dataPath))
            .Append(mlContext.MulticlassClassification.Trainers.ImageClassification())
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        var model = pipeline.Fit(data);
        mlContext.Model.Save(model, data.Schema, modelPath);
        Console.WriteLine("Model trained and saved.");
    }

    private void StartCamera()
    {
        using var capture = new OpenCvSharp.VideoCapture(0, OpenCvSharp.VideoCaptureAPIs.AVFOUNDATION); // Use AVFoundation for macOS
        frame = new Mat();

        while (true)
        {
            capture.Read(frame);
            if (!frame.Empty())
            {
                var fileName = "temp.jpg";
                Cv2.ImWrite(fileName, frame);
                PredictASL(fileName);
            }
        }
    }

    private void PredictASL(string filePath)
    {
        var model = mlContext.Model.Load(modelPath, out var schema);
        var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
        var prediction = predictor.Predict(new ImageData { Image = filePath });
        Console.WriteLine($"Predicted ASL: {prediction.PredictedLabel}");
    }

   
}