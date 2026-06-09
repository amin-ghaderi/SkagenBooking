import { BrowserRouter, Route, Routes } from 'react-router-dom'
import AppLayout from './layouts/AppLayout'
import BookingsPage from './pages/BookingsPage'
import EditBookingPage from './pages/EditBookingPage'
import HomePage from './pages/HomePage'
import NewBookingPage from './pages/NewBookingPage'

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AppLayout />}>
          <Route index element={<HomePage />} />
          <Route path="bookings" element={<BookingsPage />} />
          <Route path="bookings/new" element={<NewBookingPage />} />
          <Route path="bookings/:id/edit" element={<EditBookingPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}
