using AI_Emissions_Reduction.Model;
using Microsoft.ML;
using System.IO;

namespace AI_Emissions_Reduction.Services
{
    public class AIService
    {
        private readonly MLContext _mlContext;
        private ITransformer _trainedModel;
        private readonly DataFileService _dataFileService;

        public AIService(DataFileService dataFileService)
        {
            _mlContext = new MLContext();
            _dataFileService = dataFileService;
            _dataFileService.CreateWorkloadDataFile();
        }

        public void TrainModel()
        {
            var data = _mlContext.Data.LoadFromTextFile<WorkloadData>("workload_data.csv", hasHeader: true, separatorChar: ',');
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(WorkloadData.IsBalanced))
                            .Append(_mlContext.Transforms.Concatenate("Features", nameof(WorkloadData.WasteVolume), nameof(WorkloadData.TimeTaken)))
                            .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            _trainedModel = pipeline.Fit(data);
        }

        public bool PredictWorkload(float wasteVolume, float timeTaken)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<WorkloadData, WorkloadPrediction>(_trainedModel);
            var input = new WorkloadData { WasteVolume = wasteVolume, TimeTaken = timeTaken };
            var prediction = predictionEngine.Predict(input);

            return prediction.IsBalanced;
        }
    }
}
