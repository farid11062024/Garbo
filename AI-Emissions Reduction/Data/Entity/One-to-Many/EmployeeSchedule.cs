namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class EmployeeSchedule
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int WasteCollectionRequestId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public Employee employee { get; set; }
    }
}
