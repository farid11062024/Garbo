namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class WasteItem
    {
        public int Id { get; set; }              
        public int WasteCollectionId { get; set; }  
        public int WasteTypeId { get; set; }    
        public decimal Weight { get; set; }     
        public bool IsRecyclable { get; set; } 
        public WasteType WasteType { get; set; }
        public WasteCollection WasteCollection { get; set; }
    }
}
