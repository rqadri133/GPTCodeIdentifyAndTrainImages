using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow.Eager;

public class ASLWordGenerator
{
    // Generate all permutations of given characters with a certain length
    public static Dictionary<string,bool> GeneratePossibleWords(string letters, int minLength = 1, int maxLength = 5)
    {
        var results = new List<string>();
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        var characters = letters.ToCharArray();

        for (int length = minLength; length <= maxLength; length++)
        {
            GetCombinations(characters, "", length, results);
        }

        // Optionally filter the results with a dictionary check later
        foreach(var r in results.Distinct().ToList())
        {
            if(WordChecker.IsRealWordAsync(r).Result)
            {
                dictionary.Add(r,true);
            }
            else 
            {
                dictionary.Add(r, false);   
            }
        }        
        
        return dictionary;
    }

    // Helper recursive method to get combinations
    private static void GetCombinations(char[] characters, string prefix, int length, List<string> results)
    {
        if (length == 0)
        {
            results.Add(prefix);
            return;
        }

        foreach (var ch in characters)
        {
            GetCombinations(characters, prefix + ch, length - 1, results);
        }
    }
}
