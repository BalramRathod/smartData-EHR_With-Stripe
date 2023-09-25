using System.ComponentModel.DataAnnotations;

namespace EHR_project.Models
{
    public class TransactionHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Appointment_Id { get; set; }
        [Required]
        public string Charge_Id { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
