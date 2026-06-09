import api from './api'

export async function getRooms(propertyId) {
  const response = await api.get('/api/rooms', {
    params: { propertyId },
  })
  return response.data
}
