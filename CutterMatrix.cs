using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;





public class Cutter 
{

    public static List<string> SplitImage(string inputImagePath, string outputFolderPath, int rows, int cols)
    {
        var outputPaths = new List<string>();

        using (var image = Image.Load<Rgba32>(inputImagePath))
        {
            int cellWidth = (image.Width / cols  ) ;
            int cellHeight = (image.Height / rows) ;

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
                        outputPaths.Add(outputPath);
                    }
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