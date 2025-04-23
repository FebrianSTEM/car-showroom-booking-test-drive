using car_booking_service.Application.Models.Requests.BookingRequests;
using car_booking_service.Application.Models.Requests.UserServiceRequests;
using car_booking_service.Application.Models.Responses.BookingResponses;
using car_booking_service.Application.Models.Responses.UserServiceResponses;
using car_booking_service.Application.Services.Interfaces;
using car_booking_service.Domain.Constants;
using car_booking_service.Domain.Entities;
using car_booking_service.Domain.Exception;
using car_booking_service.Domain.Interfaces;
using Mapster;
using static car_booking_service.Domain.Enums.Enums;

namespace car_booking_service.Application.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarModelRepository _carModelRepository;
        private readonly IUserService _userService;

        public BookingService(IBookingRepository bookingRepository, 
                              ICarModelRepository carModelRepository,
                              IUserService userService)
        {
            _bookingRepository = bookingRepository;
            _carModelRepository = carModelRepository;
            _userService = userService;
        }

        public async Task<IEnumerable<BookingResponse>> GetAllBookingAsync()
        {
            IEnumerable<Booking> bookings = await _bookingRepository.GetAllAsync();

            List<string> userIds = bookings.Select(x => x.CreatedBy).Distinct().ToList();
            List<UserResponse> users = await _userService.GetUserByIds(userIds);
            var bookingResponseList = bookings.Adapt<List<BookingResponse>>();

            bookingResponseList.ForEach(booking =>
            {
                var user = users.FirstOrDefault(u => u.Id == booking.CreatedBy);
                if (user != null)
                {
                    booking.CustomerEmail = user.Email;
                    booking.CustomerPhone = user.PhoneNumber;
                }
            });

            return bookingResponseList;
        }

        public async Task<BookingResponse> GetBookingByIdAsync(int id)
        {
            Booking? booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                throw new HttpStatusCodeException((int)StatusCode.NotFound, $"Booking with ID {id} not found");

            var carModel = await _carModelRepository.GetByIdAsync(booking.CarId);
            if (carModel == null)
                throw new HttpStatusCodeException((int)StatusCode.NotFound, $"Car Model with ID {booking.CarId} not found");

            List<string> userIds = new List<string>();
            userIds.Add(booking.CreatedBy);
            List<UserResponse> users = await _userService.GetUserByIds(userIds);
            var user = users.FirstOrDefault(u => u.Id == booking.CreatedBy);
            BookingResponse result = booking.Adapt<BookingResponse>();
            result.CarModelBrand = carModel.Brand;
            result.CarModelName = carModel.Model;
            result.CarModelYear = carModel.Year;
            result.CustomerEmail = user.Email;
            result.CustomerPhone = user.PhoneNumber;

            return result;
        }

        public async Task<List<BookingResponse>> GetListAsync(GetBookingListRequest request)
        {
            var result = await _bookingRepository.GetListAsync(request.StartDate,
                                                               request.EndDate,
                                                               request.CarId,
                                                               request.CustomerName,
                                                               request.CustomerPhone,
                                                               request.CustomerEmail,
                                                               request.CarBrand,
                                                               request.CarModel,
                                                               request.CarYear);
            
            List<UserResponse> users = await _userService.GetUserByIds(result.Select(x => x.CreatedBy).ToList());

            var response = result.Adapt<List<BookingResponse>>();
            response.ForEach(x =>
            {
                var user = users.FirstOrDefault(u => u.Id == x.CreatedBy);
                if (user != null)
                {
                    x.CustomerEmail = user.Email;
                    x.CustomerPhone = user.PhoneNumber;
                }
            });

            return response;
        }

        public async Task<(List<BookingResponse>, int)> GetPaginatedAsync(GetPaginatedBookingsRequest request)
        {
            //add validation Here
            var (result, totalCount) = await _bookingRepository.GetPaginatedAsync(
                                                               request.Page,
                                                               request.PageSize,
                                                               request.StartDate,
                                                               request.EndDate,
                                                               request.CarId,
                                                               request.CustomerName,
                                                               request.CustomerPhone,
                                                               request.CustomerEmail,
                                                               request.CarBrand,
                                                               request.CarModel,
                                                               request.CarYear);

            return (result.Adapt<List<BookingResponse>>(), totalCount);
        }

        public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            CarModel carModel = await ValidateRequestCreateBookingAsync(request);
            Booking bookingEntity = request.Adapt<Booking>();         
            await _bookingRepository.AddAsync(bookingEntity);

            List<string> userIds = new List<string>();
            userIds.Add(bookingEntity.CreatedBy);

            List<UserResponse> users = await _userService.GetUserByIds(userIds);
            var user = users.FirstOrDefault(u => u.Id == bookingEntity.CreatedBy);

            var result = bookingEntity.Adapt<BookingResponse>();
            result.CarId = carModel.CarId;
            result.CarModelBrand = carModel.Brand;
            result.CarModelName = carModel.Model;
            result.CarModelYear = carModel.Year;
            result.CustomerPhone = user.PhoneNumber;
            result.CustomerEmail = user.Email;

            return result;
        }

        public async Task<BookingResponse> UpdateBookingAsync(UpdateBookingRequest request)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId)
                ?? throw new HttpStatusCodeException((int)StatusCode.UnprocessableEntity,
                    $"Booking not found for booking id {request.BookingId}");

            CarModel carModel = await ValidateCarModel(request.CarId);

            if (request.CarId != booking.CarId || request.StartBookingDate != booking.StartBookingDate || request.EndBookingDate != booking.EndBookingDate)
            {
                await ValidateBookingSlotTime(request.Adapt<CreateBookingRequest>());
            }

            request.Adapt(booking);
            await _bookingRepository.UpdateAsync(booking);

            var result = booking.Adapt<BookingResponse>();
            result.CarModelBrand = carModel.Brand;
            result.CarModelName = carModel.Model;
            result.CarModelYear = carModel.Year;

            return result;
        }

        public async Task DeleteBookingAsync(int id)
        {
            Booking booking = await ValidateDeleteBookingRequest(id);
            await _bookingRepository.DeleteAsync(booking);
        }

        public async Task<CarModel> ValidateRequestCreateBookingAsync(CreateBookingRequest request)
        {
            CarModel carModel = await ValidateCarModel(request.CarId.GetValueOrDefault(0));
            await ValidateBookingSlotTime(request);
            return carModel;
        }

        public async Task<CarModel> ValidateCarModel(int carId)
        {
            var carModel = await _carModelRepository.GetByIdAsync(carId);
            if (carModel == null)
                throw new HttpStatusCodeException((int)StatusCode.UnprocessableEntity, $"Selected Car Model with ID {carId} not found");
            return carModel;
        }

        public async Task ValidateBookingSlotTime(CreateBookingRequest request)
        {
            var existingBookings = await _bookingRepository.GetListAsync(
                request.StartBookingDate,
                request.EndBookingDate,
                request.CarId
            );

            if (existingBookings.Any())
            {
                throw new HttpStatusCodeException((int)StatusCode.UnprocessableEntity,
                    "The selected booking time overlaps with another booking.");
            }
        }


        public async Task ValidateRequestUpdateBooking(UpdateBookingRequest request)
        {
            CreateBookingRequest req = request.Adapt<CreateBookingRequest>();
            await ValidateBookingSlotTime(req);
            await ValidateCarModel(request.CarId);
        }

        public async Task<Booking> ValidateBooking(int bookingId)
        {
            Booking? booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                throw new HttpStatusCodeException((int)StatusCode.UnprocessableEntity, $"Booking not found for booking id {bookingId}");

            return booking;
        }

        public async Task<Booking> ValidateDeleteBookingRequest(int bookingId)
        {
            Booking booking = await ValidateBooking(bookingId);
            CarModel carModel = await ValidateCarModel(booking.BookingId);

            return booking;
        }
    }
}