using Microsoft.ML.Data;

namespace AI_Emissions_Reduction.Model
{
    public class WorkloadData
    {    
            [LoadColumn(0)]
            public float WasteVolume; // Tullantı həcmi

            [LoadColumn(1)]
            public float TimeTaken; // Toplama üçün tələb olunan vaxt

            [LoadColumn(2)]
            public bool IsBalanced; // İş yükləri balanslıdırmı?
    }
}
