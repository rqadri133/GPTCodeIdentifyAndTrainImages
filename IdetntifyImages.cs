using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public class Box
{
    public int Col { get; set; }
    public int Row { get; set; }
    public string ImagePath { get; set; }
    public bool IsMatchImage { get; set; }
    public string LanguageToken { get; set; }
}

public class Matrix
{
    public List<Box> Boxes { get; set; } = new List<Box>();

    public void AddBox(int col, int row, string imagePath, bool isMatchImage = false)
    {
        Boxes.Add(new Box { Col = col, Row = row, ImagePath = imagePath, IsMatchImage = isMatchImage , LanguageToken ="ReplaceASLEnglishSymbol" });
    }
}

public class ImageData
{
    [LoadColumn(0)]
    public string Image { get; set; }

    [LoadColumn(1)]
    public string Label { get; set; }

    
    
    

}

public class ImagePrediction : ImageData
{
    public float[] Score { get; set; }
    public string PredictedLabel { get; set; }
}
