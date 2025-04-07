using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_booking_service.Application.Models.Requests.BookingRequests
{
    public class GetAvailableTimeRequest
    {
        [Required(ErrorMessage = "Car id is mandatory !")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Start date is mandatory !")]
        public DateTime SelectedStartDate { get; set; }

        public DateTime? SelectedEndDate { get; set; }
    }
}
