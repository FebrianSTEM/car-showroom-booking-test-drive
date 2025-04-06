import React from 'react';
import {
  AppBar,
  Toolbar,
  Typography,
  IconButton,
  Menu,
  MenuItem,
  Box,
  Button,
  Stack,
  Avatar,
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import ShowroomLogo from '../assets/logo_showroom.png';
import { useNavigate } from 'react-router-dom';

const TopBar: React.FC = () => {
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);

  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleMenuItemClick = (path: string) => {
    console.log(`Navigate to: ${path}`);
    setAnchorEl(null);
    navigate(path);
  };

  // ✅ Check if user is logged in and get roles
  const roles: string[] = JSON.parse(localStorage.getItem('roles') || '[]');
  const isLoggedIn = !!roles;
  console.log('isLoggedin', isLoggedIn);
  const hasBookingAccess = isLoggedIn && roles?.includes('Admin');

  return (
    <AppBar position="static">
      <Toolbar>
        <Stack direction="row" alignItems="center" spacing={1} sx={{ flexGrow: 1 }}>
          <Avatar alt="Logo" src={ShowroomLogo} sx={{ width: 40, height: 40 }} />
          <Typography variant="h6" component="div">
            Febrian's Car Showroom
          </Typography>
        </Stack>

        <Box sx={{ display: { xs: 'block', sm: 'none' } }}>
          <IconButton size="large" edge="start" color="inherit" onClick={handleMenuClick}>
            <MenuIcon />
          </IconButton>
        </Box>

        {/* Desktop Menu */}
        <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
          <Button color="inherit" onClick={() => handleMenuItemClick('/carList')}>Home</Button>
          {hasBookingAccess && (
            <Button color="inherit" onClick={() => handleMenuItemClick('/myBooking')}>My Booking</Button>
          )}
          {!isLoggedIn && (
            <Button color="inherit" onClick={() => handleMenuItemClick('/login')}>Login</Button>
          )}
        </Box>

        {/* Mobile Menu */}
        <Menu anchorEl={anchorEl} open={open} onClose={handleMenuClose}>
          <MenuItem onClick={() => handleMenuItemClick('/carList')}>Home</MenuItem>
          {hasBookingAccess && (
            <MenuItem onClick={() => handleMenuItemClick('/myBooking')}>My Booking</MenuItem>
          )}
          {!isLoggedIn && (
            <MenuItem onClick={() => handleMenuItemClick('/login')}>Login</MenuItem>
          )}
        </Menu>
      </Toolbar>
    </AppBar>
  );
};

export default TopBar;