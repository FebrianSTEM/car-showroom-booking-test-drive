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

const TopBar: React.FC = () => {
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
  };

  return (
    <AppBar position="static">
      <Toolbar>
        {/* Left side: Logo + Title */}
        <Stack direction="row" alignItems="center" spacing={1} sx={{ flexGrow: 1 }}>
            <Avatar alt="Logo" src={ShowroomLogo} sx={{ width: 40, height: 40 }} />
            <Typography variant="h6" component="div">
                Febrian's Car Showroom
            </Typography>
        </Stack>

        {/* Left Side: Menu Icon */}
        <Box sx={{ display: { xs: 'block', sm: 'none' } }}>
          <IconButton
            size="large"
            edge="start"
            color="inherit"
            onClick={handleMenuClick}
          >
            <MenuIcon />
          </IconButton>
        </Box>

        {/* Right Side: Menu Items (for larger screens) */}
        <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
          <Button color="inherit" onClick={() => handleMenuItemClick('/')}>Home</Button>
          <Button color="inherit" onClick={() => handleMenuItemClick('/carList')}>Car List</Button>
          <Button color="inherit" onClick={() => handleMenuItemClick('/login')}>Login</Button>
        </Box>

        {/* Dropdown Menu (for small screens) */}
        <Menu
          anchorEl={anchorEl}
          open={open}
          onClose={handleMenuClose}
        >
          <MenuItem onClick={() => handleMenuItemClick('/')}>Home</MenuItem>
          <MenuItem onClick={() => handleMenuItemClick('/carList')}>Car List</MenuItem>
          <MenuItem onClick={() => handleMenuItemClick('/login')}>Login</MenuItem>
        </Menu>
      </Toolbar>
    </AppBar>
  );
};

export default TopBar;