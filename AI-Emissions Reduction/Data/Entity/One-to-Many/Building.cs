namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class Building
    {
        public int BuildingId { get; set; }
        public int MyProperty { get; set; }
        public string Address { get; set; } 
        public decimal WasteCapacity { get; set; } 
        public int CollectorId { get; set; }
        public Employee employee { get; set; }
    }
}
