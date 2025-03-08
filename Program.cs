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
        string modelPath =  "//Users/syedqadri/Documents/Dev/GPTCODEIDENTIFYANDTRAINIMAGES/imagerepository/image_box_model.zip";
        // assuming cols or counted from image but this also needs to be caluclated by logic
        int rows = 5;
        int cols = 5;
        string onnxModelPath = Path.Combine(Directory.GetCurrentDirectory(), "imagerepository/mobilenetv2-7.onnx");

        var croppedImagePaths = Cutter.SplitImage(inputImagePath, outputFolderPath, rows, cols);
        ASLRecognition objASL = new ASLRecognition(outputFolderPath, modelPath);
        objASL.TrainModel();
        objASL.StartCamera();    
     //   var testData = new ImageData() { Image = outputFolderPath + "//box_2_0.png" };


             
       
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
