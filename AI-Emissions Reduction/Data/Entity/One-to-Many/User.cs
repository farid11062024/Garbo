
namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class User
    {
        public int Id { get; set; }
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public Feedback feedBack { get; set; }
        public Employee employee { get; set; }
        public ICollection<WasteCollection> WasteCollectionRequests { get; set; }
        public ICollection<Notification> notifications { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public int Points { get; internal set; }
    }
}
