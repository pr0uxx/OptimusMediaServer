using Optimus.Data.Entities;

namespace Optimus.Service.HtmlSkill
{
    public class HtmlRank : UserAssessedRank
    {
        public override void StandardiseScore(decimal a, decimal b, decimal c, decimal weightA, decimal weightB, decimal weightC)
        {
            base.StandardiseScore(a, b, c, weightA, weightB, weightC);
        }
    }

    public class HtmlSkill : IHtmlSkill
    {
    }
}