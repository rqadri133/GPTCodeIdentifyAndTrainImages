using Microsoft.ML.Data;
/// <summary>
/// Convert all bytes images and Create a dictionary of millions feed dictionary
/// </summary> <summary>
/// 
/// </summary>
public class WordMapperImage 
{
 
 [LoadColumn(0)]
 
   public int LetterCodeASCII {get;set;}
    [LoadColumn(1)]
  public byte[] BitMapImageStr {get;set;}
 
    [LoadColumn(2)]
  
   public char Alpahbet {get;set;}

}

