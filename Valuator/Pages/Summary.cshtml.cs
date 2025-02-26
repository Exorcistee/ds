using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Valuator.Pages;
using StackExchange.Redis;
using System;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IConnectionMultiplexer _redis;

        public SummaryModel(ILogger<SummaryModel> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug(id);

            IDatabase db = _redis.GetDatabase();
            RedisValue rankValue = db.StringGet($"RANK-{id}");
            RedisValue similarityValue = db.StringGet($"SIMILARITY-{id}");

            if (rankValue.HasValue)
            {
                Rank = (double)rankValue;
            }
            else
            {
                Rank = 0;
            }

            if (similarityValue.HasValue)
            {
                Similarity = (double)similarityValue;
            }
            else
            {
                Similarity = 0;
            }

        }
    }
}