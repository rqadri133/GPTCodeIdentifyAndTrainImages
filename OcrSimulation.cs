using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Tesseract;
using System.Runtime.InteropServices;

public class OcrSimulation<T> where T : Image
{

    public string ExtractLetterM2(T image , string imagePath)
    {

         string tessdataPath = "/opt/homebrew/Cellar/tesseract/5.3.3/share/tessdata"; // Update with actual path
          string libraryPath = "/opt/homebrew/lib/libtesseract.dylib"; // Ensure correct path
        NativeLibrary.Load(libraryPath);

        // Manually set the environment variable for Tesseract
        Environment.SetEnvironmentVariable("TESSDATA_PREFIX", tessdataPath);
        Environment.SetEnvironmentVariable("DYLD_PRINT_LIBRARIES", libraryPath);
        // Preprocess: Convert to grayscale & binarize
        image.Mutate(x => x.Grayscale().BinaryThreshold(0.5f));

        // Save temporary file for Tesseract
        
        image.Save(imagePath);

         using (var ocrEngine = new TesseractEngine(tessdataPath, "eng", EngineMode.Default))
        {
            using (var img = Pix.LoadFromFile(imagePath))
            {
                var result = ocrEngine.Process(img);
                 return result.GetText();
            }
        }
        return "";
    }
    public string ExtractLetter(T image)
    {
        // Convert to grayscale and apply binarization
        image.Mutate(x => x.Grayscale().BinaryThreshold(0.5f));

        // Save processed image temporarily (Tesseract needs file input)
        string tempFilePath = "processed_image.png";
        image.Save(tempFilePath);

        using (var ocrEngine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
        {
            using (var page = ocrEngine.Process(Pix.LoadFromFile(tempFilePath)))
            {
                return page.GetText().Trim();
            }
        }
    }
}