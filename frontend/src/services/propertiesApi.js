import api from './api'

export async function getProperties() {
  const response = await api.get('/api/properties')
  return response.data
}
