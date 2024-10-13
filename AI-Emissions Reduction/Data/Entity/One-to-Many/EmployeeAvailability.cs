namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class EmployeeAvailability
    {
        public int Id { get; set; }         
        public int EmployeeId { get; set; }    
        public DateTime AvailableFrom { get; set; } 
        public DateTime AvailableUntil { get; set; }
        public Employee employee { get; set; }
    }
}
