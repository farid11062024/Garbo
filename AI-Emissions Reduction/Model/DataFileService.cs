namespace AI_Emissions_Reduction.Model
{
    public class DataFileService
    {
        public void CreateWorkloadDataFile()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "workload_data.csv");

            string[] csvData = new string[]
            {
                "WasteVolume,TimeTaken,IsBalanced",
                "200,30,true",
                "250,45,false",
                "180,25,true",
                "300,50,false"
            };
            File.WriteAllLines(filePath, csvData);
        }
    }
}
