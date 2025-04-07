using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_booking_service.Application.Models.Requests.BookingRequests
{
    public class CreateBookingRequest
    {
        [Required(ErrorMessage = "Car Model is Required.")]
        public int? CarId { get; set; }

        [Required(ErrorMessage = "Please Select Start Booking Date and Time.")]
        public DateTime StartBookingDate { get; set; }

        [Required(ErrorMessage = "Please Select Start Booking Date and Time.")]
        public DateTime EndBookingDate { get; set; }
    }
}
