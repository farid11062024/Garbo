using Microsoft.ML.Data;

namespace AI_Emissions_Reduction.Model
{
    public class WorkloadPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsBalanced { get; set; }
    }
}
