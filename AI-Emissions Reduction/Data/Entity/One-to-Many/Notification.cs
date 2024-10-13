namespace AI_Emissions_Reduction.Data.Entity.One_to_Many
{
    public class Notification
    {
        public int Id { get; set; }          
        public int? UserId { get; set; }      // İstifadəçi ID-si (nullable, çünki bəzi bildirişlər sürücülərə də aid ola bilər)
        public int? EmployeeId { get; set; }    // Sürücü ID-si (nullable, çünki bəzi bildirişlər istifadəçilərə də aid ola bilər)
        public string Message { get; set; }   // Bildirişin mətni
        public DateTime SentAt { get; set; }
        public User user { get; set; }
        public Employee employee { get; set; }
    }
}
