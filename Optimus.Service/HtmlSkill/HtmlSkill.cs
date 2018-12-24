using Optimus.Data.Entities;
using System;

namespace Optimus.Service.HtmlSkill
{
    public class HtmlRank : UserAssessedRank
    {
        public decimal ConvertDateTimeToInt32(DateTime dateTime)
        {
            return decimal.Parse(dateTime.ToString("yyyyMMddHHmmss"));
        }

        public override decimal StandardiseScore(decimal experience, decimal speed, decimal testScore,
            decimal weightXp, decimal weightSpeed, decimal weightTestScore)
        {
            return base.StandardiseScore(experience, speed, testScore, weightXp, weightSpeed, weightTestScore);
        }
    }

    public class HtmlSkill : IHtmlSkill
    {
    }
}