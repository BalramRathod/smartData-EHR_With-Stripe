using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EHR_project.Models
{
    public class OtpVerification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }



        public int OTP { get; set; }
        public virtual Users User { get; set; }
    }
}
