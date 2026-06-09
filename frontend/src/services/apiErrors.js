export function parseApiError(error, fallback = 'Something went wrong. Please try again.') {
  const data = error?.response?.data

  if (!data) {
    return fallback
  }

  if (typeof data.message === 'string') {
    return data.message
  }

  if (typeof data.detail === 'string') {
    return data.detail
  }

  if (data.errors && typeof data.errors === 'object') {
    const messages = Object.values(data.errors).flat()
    if (messages.length > 0) {
      return messages.join(' ')
    }
  }

  if (typeof data.title === 'string') {
    return data.title
  }

  return fallback
}
