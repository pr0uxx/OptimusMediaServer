using Optimus.Data.Enums;

namespace Optimus.Data.Entities
{
    public class UserAssessedRank
    {
        /// <summary>
        /// Example Method - Takes whole values, applies a weight and standardises
        /// </summary>
        /// <returns></returns>
        public virtual void StandardiseScore(decimal a, decimal b, decimal c, decimal weightA, decimal weightB, decimal weightC)
        {
            a = GetWeightedValue(a, weightA);
            b = GetWeightedValue(b, weightB);
            c = GetWeightedValue(c, weightC);

            StandardisedScore = (a + b + c) / 3;
        }

        public virtual void StandardiseScore(decimal a, decimal b, decimal weightA, decimal weightB)
        {
            a = GetWeightedValue(a, weightA);
            b = GetWeightedValue(b, weightB);

            StandardisedScore = (a + b) / 2;
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
        public decimal StandardisedScore { get; private set; }
        public long UserId { get; set; }
    }

    public class StandardisedTest
    {
        public void TestVoid()
        {
            var x = new UserAssessedRank();

            x.StandardiseScore(10, 8, 5, 12, 16, 14);
        }
    }
}