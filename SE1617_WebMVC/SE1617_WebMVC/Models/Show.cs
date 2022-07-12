using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SE1617_WebMVC.Models
{
    public partial class Show
    {
        public Show()
        {
            Bookings = new HashSet<Booking>();
        }

        public int ShowId { get; set; }
        public int RoomId { get; set; }
        public int FilmId { get; set; }

        public DateTime? ShowDate { get; set; }

        [Range(0, 999999, ErrorMessage = "Price must be from 0 to 999,999")]
        public decimal? Price { get; set; }
        public bool? Status { get; set; }
        public int? Slot { get; set; }

        public virtual Film Film { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
