export const initialBookingForm = {
  propertyId: '',
  roomId: '',
  checkInDate: '',
  checkOutDate: '',
  guestCount: 1,
  needsParking: false,
  isLateArrival: false,
  estimatedArrivalTime: '',
}

export function toApiDateTime(value) {
  if (!value) {
    return null
  }

  return value.length === 16 ? `${value}:00` : value
}

export function toApiTime(value) {
  if (!value) {
    return null
  }

  const timePart = value.includes('T') ? value.split('T')[1] : value
  return timePart.length === 5 ? `${timePart}:00` : timePart
}

export function toDatetimeLocalValue(isoString) {
  if (!isoString) {
    return ''
  }

  const date = new Date(isoString)
  const pad = (value) => String(value).padStart(2, '0')

  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`
}

export function estimatedArrivalToDatetimeLocal(timeOnly, checkInDate) {
  if (!timeOnly) {
    return ''
  }

  const datePart = checkInDate
    ? checkInDate.split('T')[0]
    : new Date().toISOString().split('T')[0]
  const timePart = timeOnly.substring(0, 5)

  return `${datePart}T${timePart}`
}

export function bookingToForm(booking) {
  return {
    propertyId: String(booking.propertyId),
    roomId: String(booking.roomId),
    checkInDate: toDatetimeLocalValue(booking.checkInDate),
    checkOutDate: toDatetimeLocalValue(booking.checkOutDate),
    guestCount: booking.guestCount,
    needsParking: booking.needsParking,
    isLateArrival: booking.isLateArrival,
    estimatedArrivalTime: booking.estimatedArrivalTime
      ? estimatedArrivalToDatetimeLocal(booking.estimatedArrivalTime, booking.checkInDate)
      : '',
  }
}

export function validateBookingForm(form, selectedRoom) {
  const errors = {}

  if (!form.propertyId) {
    errors.propertyId = 'Property is required.'
  }

  if (!form.roomId) {
    errors.roomId = 'Room is required.'
  }

  if (!form.checkInDate) {
    errors.checkInDate = 'Check-in is required.'
  }

  if (!form.checkOutDate) {
    errors.checkOutDate = 'Check-out is required.'
  }

  if (form.checkInDate && form.checkOutDate && new Date(form.checkOutDate) <= new Date(form.checkInDate)) {
    errors.checkOutDate = 'Check-out must be after check-in.'
  }

  if (!form.guestCount || Number(form.guestCount) < 1) {
    errors.guestCount = 'Guest count must be at least 1.'
  }

  if (selectedRoom && Number(form.guestCount) > selectedRoom.capacity) {
    errors.guestCount = `Guest count cannot exceed room capacity (${selectedRoom.capacity}).`
  }

  if (form.isLateArrival && !form.estimatedArrivalTime) {
    errors.estimatedArrivalTime = 'Estimated arrival time is required for late arrivals.'
  }

  return errors
}

export function buildCreatePayload(form) {
  return {
    propertyId: Number(form.propertyId),
    roomId: Number(form.roomId),
    checkInDate: toApiDateTime(form.checkInDate),
    checkOutDate: toApiDateTime(form.checkOutDate),
    guestCount: Number(form.guestCount),
    needsParking: form.needsParking,
    isLateArrival: form.isLateArrival,
    estimatedArrivalTime: form.isLateArrival ? toApiTime(form.estimatedArrivalTime) : null,
  }
}

export function buildUpdatePayload(form) {
  return {
    checkInDate: toApiDateTime(form.checkInDate),
    checkOutDate: toApiDateTime(form.checkOutDate),
    guestCount: Number(form.guestCount),
    needsParking: form.needsParking,
    isLateArrival: form.isLateArrival,
    estimatedArrivalTime: form.isLateArrival ? toApiTime(form.estimatedArrivalTime) : null,
  }
}

export function bookingFieldClass(hasError) {
  return [
    'mt-1 block w-full rounded-lg border px-3 py-2 text-sm shadow-sm focus:outline-none focus:ring-1',
    hasError
      ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
      : 'border-slate-300 focus:border-sky-500 focus:ring-sky-500',
  ].join(' ')
}
