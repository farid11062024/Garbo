
namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; } 
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public Feedback feedBack { get; set; }
        public Building building { get; set; }
        public ICollection<WasteCollection> WasteCollection { get; set; }
        public ICollection<User> users { get; set; }
        public ICollection<EmployeeAvailability> employeeAvailabilities { get; set; }
        public ICollection<EmployeeSchedule> employeeSchedules { get; set; }
        public ICollection<Notification> notifications { get; set; }
    }
}
