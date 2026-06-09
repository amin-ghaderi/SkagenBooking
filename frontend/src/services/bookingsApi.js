import api from './api'

export async function getBookings() {
  const response = await api.get('/api/bookings')
  return response.data
}

export async function deleteBooking(id) {
  const response = await api.delete(`/api/bookings/${id}`)
  return response.data
}

export async function createBooking(payload) {
  const response = await api.post('/api/bookings', payload)
  return response.data
}
