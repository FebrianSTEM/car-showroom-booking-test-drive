import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Card,
  CardMedia,
  CardContent,
  CircularProgress,
  Container,
  Button,
  Stack,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from '@mui/material';
import { CarModelResponse } from '../models/response/carModel/carModelResponse';
import axios from 'axios';
import carModelApiConnector from '../api/carModel/carModelApiConnector';
import bookingApiConnector from '../api/booking/bookingApiConnector';
import { toast } from 'react-toastify';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { TextField } from '@mui/material';


const CarDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [car, setCar] = useState<CarModelResponse | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [openEditDialog, setOpenEditDialog] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [openBookingDialog, setOpenBookingDialog] = useState(false);
  const [bookingStart, setBookingStart] = useState<Date | null>(null);
  const [bookingEnd, setBookingEnd] = useState<Date | null>(null);

  // Get roles from localStorage
  const roles: string[] = JSON.parse(localStorage.getItem('roles') || '[]');
  const isAdmin = roles.includes('Admin');
  const isUser = roles.includes('User');

  const [editCarData, setEditCarData] = useState({
    car_id: car?.car_id || 0,
    brand: car?.brand || '',
    model: car?.model || '',
    year: car?.year || new Date().getFullYear(),
    image_url: car?.image_url || '',
    description: car?.description || '',
  });

  useEffect(() => {
    const fetchCar = async () => {
      try {
        setLoading(true);
        const carId = parseInt(id);
        const res = await carModelApiConnector.getCarModelById(carId);
        const carData = res.data.data;
        setCar(carData);
        setEditCarData({
          car_id: carData.car_id,
          brand: carData.brand,
          model: carData.model,
          year: carData.year,
          image_url: carData.image_url,
          description: carData.description,
        });
      } catch (err) {
        setError('Failed to fetch car details');
      } finally {
        setLoading(false);
      }
    };
    fetchCar();
  }, [id]);
  

  const handleEditClick = () => {
    setOpenEditDialog(true);
  };

  const handleSubmitEdit = async () => {
    if (!editCarData.brand || !editCarData.model || !editCarData.year) {
      toast.error('Please fill in all required fields');
      return;
    }    
    try {
      const response = await carModelApiConnector.editCarById(editCarData);
      toast.success(response?.data?.message || 'Car updated successfully!');
      setOpenEditDialog(false);
      setCar({ ...car!, ...editCarData });
    } catch (err: any) {
      const errorMessage = err?.response?.data?.message || 'Failed to update car.';
      toast.error(errorMessage);
      console.error('Edit error:', err);
    }
  };  

  const handleDeleteClick  = async () => {
    setOpenDeleteDialog(true);
  };

  const handleConfirmDelete = async () => {
    try {
      const idNumber = Number.parseInt(id);
      const response = await carModelApiConnector.deleteCarById(idNumber);
      setOpenDeleteDialog(false);
  
      const successMessage =
        response?.data?.message || 'Car deleted successfully!';
      toast.success(successMessage);
      navigate('/carList');
    } catch (err: any) {
      setOpenDeleteDialog(false);
      const errorMessage = err?.response?.data?.message || 'Failed to delete car.';
      toast.error(errorMessage);
      console.error('Delete error:', err);
    }
  };

  const handleBookingClick = async() => {
      if (roles.length === 0) {
        toast.info('Please logging in before booking car for test drive');
        navigate('/login');
      }
    setOpenBookingDialog(true)
  }

  const handleBookingSubmit = async () => {
    if (!bookingStart || !bookingEnd) {
      toast.error('Please select both start and end datetime.');
      return;
    }
  
    try {
      if (!car?.car_id) {
        toast.error('Car ID is missing.');
        return;
      }
      
      const bookingReq = 
      {
        car_id : car.car_id,
        booking_date_time: bookingStart.toISOString()
      };
      console.log(bookingReq);
      const response = await bookingApiConnector.createBooking(bookingReq);
  
      toast.success(response?.data?.message || 'Booking successful!');
      setOpenBookingDialog(false);
    } catch (err: any) {
      const errorMessage = err?.response?.data?.message || 'Booking failed.';
      toast.error(errorMessage);
      console.error('Booking error:', err);
    }
  };
  
  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={5}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !car) {
    return (
      <Box textAlign="center" mt={5}>
        <Typography color="error">{error || 'Car not found'}</Typography>
      </Box>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 5 }}>
      <Button onClick={() => navigate(-1)} sx={{ mb: 2 }}>
        ← Back to List
      </Button>
        <Stack direction="row" spacing={2} sx={{ mb: 2 }}>
        {isAdmin && (
          <><Button variant="contained" color="primary" onClick={handleEditClick}>
            Edit
          </Button><Button variant="contained" color="error" onClick={handleDeleteClick}>
              Delete
            </Button></>
           )}
            <Button variant="outlined" color="secondary" onClick={handleBookingClick}>
            Booking for Test Drive
            </Button>
        </Stack>

      <Card>
        <CardMedia
          component="img"
          height="300"
          image={car.image_url}
          alt={car.model}
        />
        <CardContent>
          <Typography variant="h4" gutterBottom>
            {car.brand} {car.model}
          </Typography>
          <Typography variant="subtitle1" gutterBottom>
            Year: {car.year}
          </Typography>
          <Typography variant="body1" paragraph>
            {car.description}
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Test Drive Available: {car.is_available_for_test_drive ? 'Yes' : 'No'}
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Created By: {car.created_by}
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Created At: {new Date(car.created_at).toLocaleString()}
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Last Updated By: {car.updated_by}
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Last Updated At: {new Date(car.updated_at).toLocaleString()}
          </Typography>
        </CardContent>
      </Card>
      <Dialog open={openDeleteDialog}
              onClose={() => setOpenDeleteDialog(false)}>
      <DialogTitle>Confirm Deletion</DialogTitle>
      <DialogContent>
        <Typography>Are you sure you want to delete this car?</Typography>
      </DialogContent>
      <DialogActions>
        <Button onClick={() => setOpenDeleteDialog(false)} color="inherit">
          Cancel
        </Button>
        <Button onClick={handleConfirmDelete} color="error" variant="contained">
          Delete
        </Button>
      </DialogActions>
      </Dialog>
      <Dialog open={openBookingDialog} onClose={() => setOpenBookingDialog(false)}>
        <DialogTitle>Book Test Drive</DialogTitle>
        <DialogContent sx={{ pt: 2 }}>
          <DateTimePicker
            label="Start Time"
            value={bookingStart}
            onChange={(newValue) => setBookingStart(newValue)}
            sx={{ mb: 2 }}
          />
          <DateTimePicker
            label="End Time"
            value={bookingEnd}
            onChange={(newValue) => setBookingEnd(newValue)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenBookingDialog(false)}>Cancel</Button>
          <Button onClick={handleBookingSubmit} variant="contained">
            Submit
          </Button>
        </DialogActions>
      </Dialog>
      <Dialog open={openEditDialog} onClose={() => setOpenEditDialog(false)} fullWidth maxWidth="sm">
        <DialogTitle>Edit Car Details</DialogTitle>
        <DialogContent sx={{ pt: 2 }}>
          <Stack spacing={2}>
            <TextField
              label="Brand"
              fullWidth
              value={editCarData.brand}
              onChange={(e) => setEditCarData({ ...editCarData, brand: e.target.value })}
            />
            <TextField
              label="Model"
              fullWidth
              value={editCarData.model}
              onChange={(e) => setEditCarData({ ...editCarData, model: e.target.value })}
            />
            <TextField
              label="Year"
              fullWidth
              type="number"
              value={editCarData.year}
              onChange={(e) => setEditCarData({ ...editCarData, year: parseInt(e.target.value) })}
            />
            <TextField
              label="Image URL"
              fullWidth
              value={editCarData.image_url}
              onChange={(e) => setEditCarData({ ...editCarData, image_url: e.target.value })}
            />
            <TextField
              label="Description"
              fullWidth
              multiline
              minRows={3}
              value={editCarData.description}
              onChange={(e) => setEditCarData({ ...editCarData, description: e.target.value })}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEditDialog(false)}>Cancel</Button>
          <Button onClick={handleSubmitEdit} variant="contained">Save</Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default CarDetailPage;