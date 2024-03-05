using System.Security.Cryptography;
using System.Text;

namespace PoshtoBack.Services;

public class SeedService
{
    private static readonly string[] WordList =
    [
        "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9", "word10"
    ];

    private readonly Random _random = new();
    
    public string GenerateSeed()
    {
        var randomWords = Enumerable.Range(0, 8).Select(_ => WordList[_random.Next(WordList.Length)]).ToArray();
        return string.Join(" ", randomWords);
    }

    public string EncodeSeed(string seed)
    {
        var data = Encoding.UTF8.GetBytes(seed);
        var hashData = SHA256.HashData(data);
        var encodedPhrase = Convert.ToBase64String(hashData);

        return encodedPhrase;
    }
}