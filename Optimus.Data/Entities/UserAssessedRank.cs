using Optimus.Data.Enums;

namespace Optimus.Data.Entities
{
    public class UserAssessedRank
    {
        /// <summary>
        /// Example Method - Takes whole values, applies a weight and standardises
        /// </summary>
        /// <returns></returns>
        public virtual decimal StandardiseScore(decimal a, decimal b, decimal c, decimal weightA, decimal weightB, decimal weightC)
        {
            //a is a score of 100
            a = GetWeightedValue(a, weightA);

            //b is a score of 300
            b = GetWeightedValue(b, weightB);

            //c is a score of 500
            c = GetWeightedValue(c, weightC);

            return (a + b + c) / 3;
        }

        /// <summary>
        /// Returns a value 0-1 based on score and weight
        /// </summary>
        /// <param name="score"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public decimal GetWeightedValue(decimal score, decimal weight)
        {
            return score / weight;
        }

        public long Id { get; set; }
        public AssessmentRankName RankName { get; set; }
        public long Rank { get; set; }
        public virtual int RankValueA { get; set; }
        public virtual int RankValueB { get; set; }
        public virtual int RankValueC { get; set; }
        public long UserId { get; set; }
    }
}