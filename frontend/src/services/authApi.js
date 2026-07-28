import api from './api'

export async function register(email, password) {
  const response = await api.post('/register', { email, password })
  return response.data
}

export async function login(email, password) {
  const response = await api.post('/login?useCookies=true', { email, password })
  return response.data
}

export async function logout() {
  const response = await api.post('/logout', {})
  return response.data
}

export async function getCurrentUser() {
  const response = await api.get('/manage/info')
  return response.data
}
