using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using Tesseract;
using System.Collections.Generic;
using IronOcr;


public class ImageMapper 
{
    public string imagePath { get; set; }
    public char Alphabet {get; set; }


}

public class Cutter 
{




  
 public static string GetTextFromImageIron(string currentImage)
 {
        using (var image = Image.Load(currentImage))
        {
            var ocrProcessor = new OCRProcessor<Image>();
            string letter = ocrProcessor.ExtractLetterM2(image,currentImage);

             return letter;
         }
      return "";

 }
 
  public static string GetTextFromImage(string imagePath) 
  {
       string firstcut = String.Empty;
      string tessdataPath = "/opt/homebrew/share/tessdata";
        try
        {
            using (var engine = new TesseractEngine(tessdataPath, "eng", EngineMode.Default))
            {
                using (var image = Pix.LoadFromFile(imagePath))
                {
                    using (var page = engine.Process(image))
                    {
                        string extractedText = page.GetText();
                        Console.WriteLine("Extracted Text:");
                         return extractedText  ;                  

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        return firstcut;

  }
    public static Dictionary<string,ImageMapper> SplitImage(string inputImagePath, string outputFolderPath, int rows, int cols)
    {
        var outputPaths = new Dictionary<string,ImageMapper>();

        using (var image = Image.Load<Rgba32>(inputImagePath))
        {
            int cellWidth = (image.Width / cols  ) ;
            int cellHeight = (image.Height / rows) ;
            int num = 65 ; 
            Directory.CreateDirectory(outputFolderPath);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var rect = new Rectangle(col * cellWidth, row * cellHeight, cellWidth, cellHeight);

                    using (var croppedImage = image.Clone(ctx => ctx.Crop(rect)))
                    {
                        string outputPath = Path.Combine(outputFolderPath, $"box_{row}_{col}.png");
                        croppedImage.Save(outputPath, new PngEncoder());
                        var imageMapper = new ImageMapper(){ Alphabet = (char) num ,  imagePath = outputPath };
                        outputPaths.Add( row.ToString() + col.ToString() , imageMapper) ;
                    }
                    num = num + 1;
                }
            }
        }

        return outputPaths;
    }
 public static void SplitImageIntoMatrix(string inputImagePath, string outputFolderPath, int rows, int cols)
    {

        using (var image = Image.Load<Rgba32>(inputImagePath))
        {
            int cellWidth = image.Width / cols;
            int cellHeight = image.Height / rows;

            Directory.CreateDirectory(outputFolderPath);    

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var rect = new Rectangle(col * cellWidth, row * cellHeight, cellWidth, cellHeight);

                    using (var croppedImage = image.Clone(ctx => ctx.Crop(rect)))
                    {
                        string outputPath = Path.Combine(outputFolderPath, $"box_{row}_{col}.png");
                        croppedImage.Save(outputPath, new PngEncoder());
                        Console.WriteLine($"Saved: {outputPath}");
                    }
                }
            }
        }
    }

    public static Matrix LoadMatrixFromFolder(string folderPath, int rows, int cols)
    {
        var matrix = new Matrix();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                string imagePath = Path.Combine(folderPath, $"box_{row}_{col}.png");
                matrix.AddBox(col, row, imagePath);
            }
        }
        return matrix;
    }
}