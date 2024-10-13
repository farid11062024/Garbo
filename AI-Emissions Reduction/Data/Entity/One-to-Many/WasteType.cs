namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class WasteType
    {
        public int Id { get; set; }        
        public string Name { get; set; }   
        public ICollection<WasteCollection> WasteCollections { get; set; }
        public ICollection<WasteItem> WasteItems { get; set; } 
        public decimal RecyclingEnergy { get; internal set; }
    }
}
