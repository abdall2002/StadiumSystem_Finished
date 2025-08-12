using System.ComponentModel.DataAnnotations;

namespace StatiumSystem.Models
{
    public class ReservationDTO
    {
        [Required]
        public int StadiumId { get; set; }


        [Required]
        public DateTime ReservationDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}
