namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class Feedback
    {
        public int Id { get; set; }         
        public int UserId { get; set; }    
        public int EmployeeId { get; set; }   
        public int Rating { get; set; }    
        public string Comments { get; set; } 
        public DateTime CreatedAt { get; set; }
        public User user { get; set; }
        public Employee employee { get; set; }
    }
}
