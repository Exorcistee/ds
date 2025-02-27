using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using System.Text.RegularExpressions;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnectionMultiplexer _redis;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost(string text)
    {

        if (string.IsNullOrEmpty(text))
        {
            return Redirect("index");
        }

        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();
        string textKey = "TEXT-" + id;

        var db = _redis.GetDatabase();

        // Вычисление и сохранение ранга
        string rankKey = "RANK-" + id;
        double rank = CalculateRank(text);
        db.StringSet(rankKey, rank);

        // Проверка на похожесть
        string similarityKey = "SIMILARITY-" + id;
        double similarity = CalculateSimilarity(text, db);
        db.StringSet(similarityKey, similarity);

        db.StringSet(textKey, text);

        return Redirect($"summary?id={id}");
    }

    private double CalculateRank(string text)   
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }
        double count = 0;

        foreach (char ch in text)
        {
            if (char.IsLetter(ch))
            {
                count++;
            }
        }

        return Math.Round(1 - count / text.Length, 2, MidpointRounding.AwayFromZero);
    }

    private double CalculateSimilarity(string text, IDatabase db)
    {
        IServer server = _redis.GetServer(_redis.GetEndPoints().First());

        IEnumerable<RedisKey> keys = server.Keys(pattern: "TEXT-*");
        foreach (RedisKey key in keys)
        {
            string existingText = db.StringGet(key);
            if (existingText == text)
            {
                return 1; 
            }
        }
        return 0;
    }
}


