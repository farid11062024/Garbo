namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WasteCollectionId { get; set; }
        public decimal Amount { get; set; }   
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public User User { get; set; }
        public object WasteCollection { get; internal set; }
    }
}
