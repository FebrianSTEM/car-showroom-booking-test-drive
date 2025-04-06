import React, { useEffect, useState } from 'react';
import { TextField, Button, Grid, Paper, Typography } from '@mui/material';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import dayjs from 'dayjs';
import { CarBookingResponse } from '../models/response/booking/bookingResponse';
import bookingApiConnector from '../api/booking/bookingApiConnector';

const MyBookingPage: React.FC = () => {
  const [filter, setFilter] = useState<GetPagingBookingRequest>({
    start_date: dayjs().startOf('month').toISOString(),
    end_date: dayjs().endOf('month').toISOString(),
    customer_name: '',
    customer_phone: '',
    customer_email: '',
    car_brand: '',
    car_model: '',
    car_year: undefined
  });

  const [bookings, setBookings] = useState<CarBookingResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalData, setTotalData] = useState(0);

  const fetchBookings = async () => {
    setLoading(true);
    try {
      const response = await bookingApiConnector.getPagingBooking({
        ...filter,
        page_index: pageIndex,
        page_size: pageSize
      } as any);

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

  const columns: GridColDef[] = [
    { field: 'booking_id', headerName: 'Booking ID', width: 100 },
    { field: 'car_model_brand', headerName: 'Brand', width: 120 },
    { field: 'car_model_name', headerName: 'Model', width: 120 },
    { field: 'car_model_year', headerName: 'Year', width: 90 },
    { field: 'booking_date_time', headerName: 'Booking Time', width: 180,
      valueFormatter: (params) => dayjs(params.value).format('YYYY-MM-DD HH:mm')
    },
    { field: 'customer_name', headerName: 'Customer Name', width: 150 },
    { field: 'customer_email', headerName: 'Email', width: 180 },
    { field: 'customer_phone', headerName: 'Phone', width: 140 },
  ];

  return (
    <Paper sx={{ padding: 3 }}>
      <Typography variant="h5" gutterBottom>
        My Bookings
      </Typography>

      <Grid container spacing={2} mb={2}>
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
    </Paper>
  );
};

export default MyBookingPage;