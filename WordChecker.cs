using System;
using System.Net.Http;
using System.Threading.Tasks;

public class WordChecker
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<bool> IsRealWordAsync(string word)
    {
        try
        {
            var url = $"https://api.dictionaryapi.dev/api/v2/entries/en/{word}";
            var response = await client.GetAsync(url);

            return response.IsSuccessStatusCode; // 200 if valid word
        }
        catch (Exception)
        {
            return false; // In case of any error, consider it invalid
        }
    }
}
