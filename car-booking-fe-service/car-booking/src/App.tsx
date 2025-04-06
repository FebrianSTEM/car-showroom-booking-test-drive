import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './components/LoginPage';
import RegistrationPage from './components/RegistrationPage';
import './App.css';
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import CarModelListPage from './components/CarModelListPage';
import CarDetailPage from './components/CarDetailPage';
import TopBar from './components/topBar';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import MyBookingPage from './components/MyBookingPage';

function App() {
  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <TopBar />
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegistrationPage />} />
          <Route path="/carList" element={<CarModelListPage />} />
          <Route path="/carDetail/:id" element={<CarDetailPage />} />
          <Route path="/myBooking" element={<MyBookingPage />} />
          <Route path="/" element={<Navigate to="/carList" replace />} />
        </Routes>
      <ToastContainer position="top-right" autoClose={3000} />
    </LocalizationProvider>
  );
}

export default App;