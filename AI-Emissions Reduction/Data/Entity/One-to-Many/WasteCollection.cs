namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class WasteCollection
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public int PaymentId { get; set; }
        public int WasteTypeId { get; set; }
        public DateTime CollectionDate { get; set; }
        public string Status { get; set; }
        public User user { get; set; }
        public WasteType WasteType { get; set; }
        public ICollection<WasteItem> WasteItems { get; set; }

        public Employee employee { get; set; }

    }
}
