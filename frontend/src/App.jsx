import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './auth/AuthContext'
import ProtectedRoute from './auth/ProtectedRoute'
import AppLayout from './layouts/AppLayout'
import BookingsPage from './pages/BookingsPage'
import EditBookingPage from './pages/EditBookingPage'
import HomePage from './pages/HomePage'
import LoginPage from './pages/LoginPage'
import NewBookingPage from './pages/NewBookingPage'
import RegisterPage from './pages/RegisterPage'

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<AppLayout />}>
            <Route index element={<HomePage />} />
            <Route path="login" element={<LoginPage />} />
            <Route path="register" element={<RegisterPage />} />
            <Route path="bookings" element={<BookingsPage />} />
            <Route
              path="bookings/new"
              element={
                <ProtectedRoute>
                  <NewBookingPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="bookings/:id/edit"
              element={
                <ProtectedRoute>
                  <EditBookingPage />
                </ProtectedRoute>
              }
            />
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}
