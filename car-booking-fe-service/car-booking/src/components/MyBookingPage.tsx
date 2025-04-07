import React, { useEffect, useState } from 'react';
import { TextField, Button, Grid, Paper, Typography, DialogContent, DialogActions, Dialog, DialogTitle, DialogContentText } from '@mui/material';
import { DataGrid, GridActionsCellItem, GridColDef } from '@mui/x-data-grid';
import dayjs from 'dayjs';
import { CarBookingResponse } from '../models/response/booking/bookingResponse';
import bookingApiConnector from '../api/booking/bookingApiConnector';
import { GetPagingBookingRequest } from '../models/request/booking/getPagingBookingRequest';
import { IconButton } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { toast } from 'react-toastify';

const MyBookingPage: React.FC = () => {
  const [filter, setFilter] = useState<GetPagingBookingRequest>({
    start_date: dayjs().hour(8).minute(0).second(0).millisecond(0).toISOString(),
    end_date: dayjs().hour(17).minute(0).second(0).millisecond(0).toISOString(),
    customer_name: '',
    customer_phone: '',
    customer_email: '',
    car_brand: '',
    car_model: '',
    car_year: undefined
  });
  
  const roles: string[] = JSON.parse(localStorage.getItem('roles') || '[]');
  const isLoggedIn = Array.isArray(roles) && roles.length > 0;
  const isAdmin = isLoggedIn && roles?.includes('Admin');

  const [bookings, setBookings] = useState<CarBookingResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalData, setTotalData] = useState(0);
  
  const [openEdit, setOpenEdit] = useState(false);
  const [selectedBooking, setSelectedBooking] = useState<CarBookingResponse | null>(null);

  const [openConfirmDelete, setOpenConfirmDelete] = useState(false);
  const [bookingToDelete, setBookingToDelete] = useState<number | null>(null);

  


  const fetchBookings = async () => {
    setLoading(true);
    try {
      // Remove empty/null/undefined params
      const queryParams: any = {
        ...filter,
        page: pageIndex,
        page_size: pageSize
      };
  
      // Clean up queryParams to remove any empty string, null, or undefined
      Object.keys(queryParams).forEach((key) => {
        const value = queryParams[key];
        const isEmptyString = typeof value === 'string' && value.trim() === '';
        if (value === null || value === undefined || isEmptyString) {
          delete queryParams[key];
        }
      });
  
      const response = await bookingApiConnector.getPagingBooking(queryParams);
      console.log(response.data);
      setBookings(response.data || []);
      setTotalData(response.total_data || 0);
    } catch (error) {
      console.error('Failed to fetch bookings', error);
    } finally {
      setLoading(false);
    }
  };
  

  useEffect(() => {
    fetchBookings();
  }, [pageIndex, pageSize]);

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFilter({ ...filter, [e.target.name]: e.target.value });
  };

  const handleEdit = (row: CarBookingResponse) => {
    setSelectedBooking(row);
    
  };

  const handleEditClick = (booking: CarBookingResponse) => {
    setSelectedBooking(booking);
    setOpenEdit(true);
  };
  
  const confirmDelete = (bookingId: number) => {
    setBookingToDelete(bookingId);
    setOpenConfirmDelete(true);
  };
  
  const handleDelete = async () => {
    if (!bookingToDelete) return;
  
    try {
      await bookingApiConnector.deleteBooking(bookingToDelete);
      toast.success('Booking deleted successfully!');
      fetchBookings();
    } catch (err: any) {
      const errorMsg = err?.response?.data?.message || 'Failed to delete booking';
      toast.error(errorMsg);
    } finally {
      setOpenConfirmDelete(false);
      setBookingToDelete(null);
    }
  };
  
  
  const columns: GridColDef[] = [
    { field: 'booking_id', headerName: 'Booking ID', width: 100 },
    { field: 'car_model_brand', headerName: 'Brand', width: 120 },
    { field: 'car_model_name', headerName: 'Model', width: 120 },
    { field: 'car_model_year', headerName: 'Year', width: 90 },
    { field: 'start_booking_date', headerName: 'Start Booking Time', width: 180 },
    { field: 'end_booking_date', headerName: 'End Booking Time', width: 180 },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 100,
      getActions: (params) => [
        <GridActionsCellItem icon={<EditIcon />} label="Edit" onClick={() => handleEditClick(params.row)} />,
        <GridActionsCellItem icon={<DeleteIcon />} label="Delete" onClick={() => confirmDelete(params.row.booking_id)} />,
      ],
    }
  ];

  return (
    <Paper sx={{ padding: 3 }}>
      <Typography variant="h5" gutterBottom>
        My Bookings
      </Typography>
        <Grid container spacing={2} mb={2}>
          {isAdmin && (
          <>
            <Grid item xs={12} sm={6} md={3}>
              <TextField
                label="Customer Name"
                name="customer_name"
                value={filter.customer_name}
                onChange={handleFilterChange}
                fullWidth
              />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <TextField
                label="Customer Phone"
                name="customer_phone"
                value={filter.customer_phone}
                onChange={handleFilterChange}
                fullWidth
              />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <TextField
                label="Customer Email"
                name="customer_email"
                value={filter.customer_email}
                onChange={handleFilterChange}
                fullWidth
              />
            </Grid>
          </>
        )}
        <Grid item xs={12} sm={6} md={3}>
          <TextField
            label="Car Brand"
            name="car_brand"
            value={filter.car_brand}
            onChange={handleFilterChange}
            fullWidth
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <TextField
            label="Car Model"
            name="car_model"
            value={filter.car_model}
            onChange={handleFilterChange}
            fullWidth
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <TextField
            label="Car Year"
            name="car_year"
            value={filter.car_year || ''}
            onChange={(e) => setFilter({ ...filter, car_year: parseInt(e.target.value) || undefined })}
            type="number"
            fullWidth
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <TextField
            label="Start Date"
            type="date"
            name="start_date"
            value={dayjs(filter.start_date).format('YYYY-MM-DD')}
            onChange={(e) =>
              setFilter({ ...filter, start_date: dayjs(e.target.value).startOf('day').toISOString() })
            }
            InputLabelProps={{ shrink: true }}
            fullWidth
          />
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <TextField
            label="End Date"
            type="date"
            name="end_date"
            value={dayjs(filter.end_date).format('YYYY-MM-DD')}
            onChange={(e) =>
              setFilter({ ...filter, end_date: dayjs(e.target.value).endOf('day').toISOString() })
            }
            InputLabelProps={{ shrink: true }}
            fullWidth
          />
        </Grid>
        <Grid item xs={12}>
          <Button variant="contained" onClick={fetchBookings}>Filter</Button>
        </Grid>
      </Grid>

      <DataGrid
        rows={bookings}
        columns={columns}
        getRowId={(row) => row.booking_id}
        autoHeight
        pagination
        paginationMode="server"
        rowCount={totalData}
        pageSize={pageSize}
        page={pageIndex - 1}
        onPageChange={(newPage) => setPageIndex(newPage + 1)}
        onPageSizeChange={(newSize) => setPageSize(newSize)}
        loading={loading}
      />
      <Dialog open={openEdit} onClose={() => setOpenEdit(false)} fullWidth maxWidth="sm">
  <DialogTitle>Edit Booking</DialogTitle>
  <DialogContent dividers>
    <Grid container spacing={2}>
      <Grid item xs={12}>
        <TextField
          label="Start Booking Date"
          type="datetime-local"
          fullWidth
          value={dayjs(selectedBooking?.start_booking_date).format('YYYY-MM-DDTHH:mm')}
          onChange={(e) =>
            setSelectedBooking({
              ...selectedBooking!,
              start_booking_date: dayjs(e.target.value).tz('Asia/Jakarta').format('YYYY-MM-DDTHH:mm:ss'),
            })

          }
        />
      </Grid>
      <Grid item xs={12}>
        <TextField
          label="End Booking Date"
          type="datetime-local"
          fullWidth
          value={dayjs(selectedBooking?.end_booking_date).format('YYYY-MM-DDTHH:mm')}
          onChange={(e) =>
            setSelectedBooking({
              ...selectedBooking!,
              end_booking_date: dayjs(e.target.value).tz('Asia/Jakarta').format('YYYY-MM-DDTHH:mm:ss'),
            })
          }
        />
      </Grid>
    </Grid>
  </DialogContent>
      <DialogActions>
        <Button onClick={() => setOpenEdit(false)}>Cancel</Button>
        <Button
          variant="contained"
          onClick={async () => {
            if (selectedBooking) {
              try {
                const response = await bookingApiConnector.updateBooking(selectedBooking);
                toast.success(response.message || 'Booking updated successfully!');
                setOpenEdit(false);
                fetchBookings();
              } catch (err: any) {
                const errorMessage = err?.response?.data?.message || 'Failed to update booking';
                toast.error(errorMessage);
                console.error('Failed to update booking', err);
              }
            }
          }}
        >
          Save
        </Button>
      </DialogActions>
    </Dialog>

    <Dialog
      open={openConfirmDelete}
      onClose={() => setOpenConfirmDelete(false)}
    >
      <DialogTitle>Confirm Delete</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Are you sure you want to delete this booking? This action cannot be undone.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={() => setOpenConfirmDelete(false)} color="inherit">
          Cancel
        </Button>
        <Button onClick={handleDelete} color="error" variant="contained">
          Delete
        </Button>
      </DialogActions>
    </Dialog>

    </Paper>
  );
};

export default MyBookingPage;