import React, { useEffect, useState } from 'react';
import {
  Box,
  Grid,
  Card,
  CardContent,
  CardMedia,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  DialogContentText,
  Typography,
  TextField,
  Button,
  Pagination,
  Container,
} from '@mui/material';

import { CarModelResponse } from '../models/response/carModel/carModelResponse';
import { PaginatedResponse } from '../models/paginatedResponse';
import carModelApiConnector from '../api/carModel/carModelApiConnector';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';

const CarListPage: React.FC = () => {
  const navigate = useNavigate();
  const [cars, setCars] = useState<CarModelResponse[]>([]);
  const [page, setPage] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(1);
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [newCar, setNewCar] = useState({
    brand: '',
    model: '',
    year: '',
    image_url: '',
    description: '',
  });

  const roles: string[] = JSON.parse(localStorage.getItem('roles') || '[]');
  const isAdmin = roles.includes('Admin');
  console.log('Admin', isAdmin);

  const [filters, setFilters] = useState({
    brand: '',
    model: '',
    year: '',
    description: '',
  });

  const pageSize = 5;

  const fetchCars = async () => {
    try {
      const req = {
        brand: filters.brand || undefined,
        model: filters.model || undefined,
        year: filters.year ? parseInt(filters.year) : undefined,
        description: filters.description || undefined,
        page,
        page_size: pageSize,
      };
      const res: PaginatedResponse<CarModelResponse[]> = await carModelApiConnector.getPaginatedCarModels(req);
      console.log("API response:", res);
      setCars(res.data.data);
      setTotalPages(res.data.total_pages);
    } catch (error) {
      console.error('Error fetching cars:', error);
    }
  };

  useEffect(() => {
    fetchCars();
  }, [page]);

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFilters({ ...filters, [e.target.name]: e.target.value });
  };

  const handleFilterSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    fetchCars();
  };

  return (
    <Container maxWidth="lg">
      <Typography variant="h4" gutterBottom sx={{ mt: 4 }}>
        Car Lists
      </Typography>

      {isAdmin && (
        <Box textAlign="right" sx={{ mb: 2 }}>
          <Button variant="contained" onClick={() => setOpenAddDialog(true)}>
            + Add Car
          </Button>
        </Box>
      )}

      <Box component="form" onSubmit={handleFilterSubmit} sx={{ mb: 4 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={3}>
            <TextField label="Brand" name="brand" fullWidth value={filters.brand} onChange={handleFilterChange} />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField label="Model" name="model" fullWidth value={filters.model} onChange={handleFilterChange} />
          </Grid>
          <Grid item xs={12} sm={2}>
            <TextField label="Year" name="year" type="number" fullWidth value={filters.year} onChange={handleFilterChange} />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField label="Description" name="description" fullWidth value={filters.description} onChange={handleFilterChange} />
          </Grid>
          <Grid item xs={12} sm={1}>
            <Button type="submit" variant="contained" fullWidth sx={{ height: '100%' }}>Filter</Button>
          </Grid>
        </Grid>
      </Box>

      <Grid container spacing={3}>
  {cars.map((car) => (
    <Grid item xs={12} sm={6} md={4} key={car.car_id}>
      <Card
        onClick={() => navigate(`/carDetail/${car.car_id}`)}
        sx={{
          height: 300, // Fixed card height
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
          cursor: 'pointer',
          transition: '0.2s',
          '&:hover': { boxShadow: 6 },
        }}
      >
        <CardMedia
          component="img"
          height="140"
          image={car.image_url}
          alt={car.model}
        />
        <CardContent sx={{ flexGrow: 1 }}>
          <Typography variant="h6" gutterBottom>
            {car.brand} {car.model}
          </Typography>
          <Typography color="textSecondary">Year: {car.year}</Typography>
          <Typography
            variant="body2"
            sx={{
              mt: 1,
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              display: '-webkit-box',
              WebkitLineClamp: 3,
              WebkitBoxOrient: 'vertical',
              wordBreak: 'break-word',
              maxWidth: '20ch'
            }}
          >
            {car.description}
          </Typography>
        </CardContent>
      </Card>
    </Grid>
  ))}
</Grid>


      <Box display="flex" justifyContent="center" mt={4}>
        <Pagination
          count={totalPages}
          page={page}
          onChange={(e, value) => setPage(value)}
          color="primary"
        />
      </Box>

      <Dialog open={openAddDialog} onClose={() => setOpenAddDialog(false)} fullWidth>
        <DialogTitle>Add New Car</DialogTitle>
        <DialogContent>
          <DialogContentText sx={{ mb: 2 }}>
            Fill out the form to add a new car.
          </DialogContentText>

          <TextField
            margin="dense"
            label="Brand"
            fullWidth
            value={newCar.brand}
            onChange={(e) => setNewCar({ ...newCar, brand: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Model"
            fullWidth
            value={newCar.model}
            onChange={(e) => setNewCar({ ...newCar, model: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Year"
            type="number"
            fullWidth
            value={newCar.year}
            onChange={(e) => setNewCar({ ...newCar, year: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Image URL"
            fullWidth
            value={newCar.image_url}
            onChange={(e) => setNewCar({ ...newCar, image_url: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Description"
            fullWidth
            multiline
            rows={3}
            value={newCar.description}
            onChange={(e) => setNewCar({ ...newCar, description: e.target.value })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenAddDialog(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={async () => {
              try {
                const res = await carModelApiConnector.createCarModel({
                  brand: newCar.brand,
                  model: newCar.model,
                  year: parseInt(newCar.year),
                  image_url: newCar.image_url,
                  description: newCar.description,
                });
                console.log('API Create Car', res);
                const successMessage = res?.data.message || 'Car created successfully!';
                toast.success(successMessage);
            
                setOpenAddDialog(false);
                setNewCar({
                  brand: '',
                  model: '',
                  year: '',
                  image_url: '',
                  description: '',
                });
            
                fetchCars(); // refresh list
              } catch (err: any) {
                const errorMessage = err?.response?.data?.message || 'Failed to create car';
                if(errorMessage === 'Invalid or expired token.'){
                  localStorage.removeItem('roles');
                  localStorage.removeItem('token');
                }
                toast.error(errorMessage);
                console.error('Failed to create car:', err);
              }
            }}
          >
            Save
          </Button>
        </DialogActions>
      </Dialog>

    </Container>
  );
};

export default CarListPage;