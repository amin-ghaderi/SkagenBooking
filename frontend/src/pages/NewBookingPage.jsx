import { useEffect, useMemo, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import ErrorAlert from '../components/ErrorAlert'
import LoadingSpinner from '../components/LoadingSpinner'
import { parseApiError } from '../services/apiErrors'
import { createBooking } from '../services/bookingsApi'
import { getProperties } from '../services/propertiesApi'
import { getRooms } from '../services/roomsApi'

const initialForm = {
  propertyId: '',
  roomId: '',
  checkInDate: '',
  checkOutDate: '',
  guestCount: 1,
  needsParking: false,
  isLateArrival: false,
  estimatedArrivalTime: '',
}

function toApiDateTime(value) {
  if (!value) {
    return null
  }

  return value.length === 16 ? `${value}:00` : value
}

function toApiTime(value) {
  if (!value) {
    return null
  }

  const timePart = value.includes('T') ? value.split('T')[1] : value
  return timePart.length === 5 ? `${timePart}:00` : timePart
}

function validateForm(form, selectedRoom) {
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

function fieldClass(hasError) {
  return [
    'mt-1 block w-full rounded-lg border px-3 py-2 text-sm shadow-sm focus:outline-none focus:ring-1',
    hasError
      ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
      : 'border-slate-300 focus:border-sky-500 focus:ring-sky-500',
  ].join(' ')
}

export default function NewBookingPage() {
  const navigate = useNavigate()
  const [form, setForm] = useState(initialForm)
  const [properties, setProperties] = useState([])
  const [rooms, setRooms] = useState([])
  const [loadingProperties, setLoadingProperties] = useState(true)
  const [loadingRooms, setLoadingRooms] = useState(false)
  const [loadError, setLoadError] = useState('')
  const [submitError, setSubmitError] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [showValidation, setShowValidation] = useState(false)

  const selectedRoom = useMemo(
    () => rooms.find((room) => String(room.id) === String(form.roomId)),
    [rooms, form.roomId],
  )

  const validationErrors = useMemo(() => validateForm(form, selectedRoom), [form, selectedRoom])
  const isFormValid = Object.keys(validationErrors).length === 0

  useEffect(() => {
    let active = true

    ;(async () => {
      try {
        const data = await getProperties()
        if (active) {
          setProperties(data)
          setLoadError('')
        }
      } catch (error) {
        if (active) {
          setLoadError(parseApiError(error, 'Failed to load properties.'))
        }
      } finally {
        if (active) {
          setLoadingProperties(false)
        }
      }
    })()

    return () => {
      active = false
    }
  }, [])

  useEffect(() => {
    if (!form.propertyId) {
      return undefined
    }

    let active = true

    ;(async () => {
      try {
        const data = await getRooms(form.propertyId)
        if (active) {
          setRooms(data)
          setLoadError('')
        }
      } catch (error) {
        if (active) {
          setRooms([])
          setLoadError(parseApiError(error, 'Failed to load rooms.'))
        }
      } finally {
        if (active) {
          setLoadingRooms(false)
        }
      }
    })()

    return () => {
      active = false
    }
  }, [form.propertyId])

  const updateField = (name, value) => {
    setSubmitError('')
    setForm((current) => ({ ...current, [name]: value }))
  }

  const handlePropertyChange = (propertyId) => {
    setSubmitError('')
    setRooms([])
    setLoadingRooms(Boolean(propertyId))
    setForm((current) => ({
      ...current,
      propertyId,
      roomId: '',
    }))
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setShowValidation(true)
    setSubmitError('')

    if (!isFormValid) {
      return
    }

    setSubmitting(true)

    try {
      await createBooking({
        propertyId: Number(form.propertyId),
        roomId: Number(form.roomId),
        checkInDate: toApiDateTime(form.checkInDate),
        checkOutDate: toApiDateTime(form.checkOutDate),
        guestCount: Number(form.guestCount),
        needsParking: form.needsParking,
        isLateArrival: form.isLateArrival,
        estimatedArrivalTime: form.isLateArrival ? toApiTime(form.estimatedArrivalTime) : null,
      })

      navigate('/bookings')
    } catch (error) {
      const status = error.response?.status
      const fallback =
        status === 409
          ? 'Booking could not be created due to a conflict.'
          : 'Failed to create booking. Please check the form and try again.'

      setSubmitError(parseApiError(error, fallback))
    } finally {
      setSubmitting(false)
    }
  }

  if (loadingProperties) {
    return <LoadingSpinner />
  }

  return (
    <section className="space-y-6">
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-2xl font-semibold text-slate-900">New Booking</h1>
          <p className="mt-1 text-slate-500">Create a new reservation</p>
        </div>
        <Link
          to="/bookings"
          className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        >
          Back to Bookings
        </Link>
      </div>

      {loadError && <ErrorAlert message={loadError} onDismiss={() => setLoadError('')} />}
      {submitError && <ErrorAlert message={submitError} onDismiss={() => setSubmitError('')} />}

      <form
        onSubmit={handleSubmit}
        className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
        noValidate
      >
        <div className="grid gap-6 md:grid-cols-2">
          <div>
            <label htmlFor="propertyId" className="block text-sm font-medium text-slate-700">
              Property
            </label>
            <select
              id="propertyId"
              value={form.propertyId}
              onChange={(event) => handlePropertyChange(event.target.value)}
              className={fieldClass(showValidation && validationErrors.propertyId)}
            >
              <option value="">Select a property</option>
              {properties.map((property) => (
                <option key={property.id} value={property.id}>
                  {property.name} ({property.city})
                </option>
              ))}
            </select>
            {showValidation && validationErrors.propertyId && (
              <p className="mt-1 text-sm text-red-600">{validationErrors.propertyId}</p>
            )}
          </div>

          <div>
            <label htmlFor="roomId" className="block text-sm font-medium text-slate-700">
              Room
            </label>
            <select
              id="roomId"
              value={form.roomId}
              onChange={(event) => updateField('roomId', event.target.value)}
              disabled={!form.propertyId || loadingRooms}
              className={fieldClass(showValidation && validationErrors.roomId)}
            >
              <option value="">
                {!form.propertyId
                  ? 'Select a property first'
                  : loadingRooms
                    ? 'Loading rooms...'
                    : 'Select a room'}
              </option>
              {rooms.map((room) => (
                <option key={room.id} value={room.id}>
                  {room.name} (Capacity {room.capacity} • {room.nightlyRate} {room.currency}/night)
                </option>
              ))}
            </select>
            {showValidation && validationErrors.roomId && (
              <p className="mt-1 text-sm text-red-600">{validationErrors.roomId}</p>
            )}
          </div>

          <div>
            <label htmlFor="checkInDate" className="block text-sm font-medium text-slate-700">
              Check In
            </label>
            <input
              id="checkInDate"
              type="datetime-local"
              value={form.checkInDate}
              onChange={(event) => updateField('checkInDate', event.target.value)}
              className={fieldClass(showValidation && validationErrors.checkInDate)}
            />
            {showValidation && validationErrors.checkInDate && (
              <p className="mt-1 text-sm text-red-600">{validationErrors.checkInDate}</p>
            )}
          </div>

          <div>
            <label htmlFor="checkOutDate" className="block text-sm font-medium text-slate-700">
              Check Out
            </label>
            <input
              id="checkOutDate"
              type="datetime-local"
              value={form.checkOutDate}
              onChange={(event) => updateField('checkOutDate', event.target.value)}
              className={fieldClass(showValidation && validationErrors.checkOutDate)}
            />
            {showValidation && validationErrors.checkOutDate && (
              <p className="mt-1 text-sm text-red-600">{validationErrors.checkOutDate}</p>
            )}
          </div>

          <div>
            <label htmlFor="guestCount" className="block text-sm font-medium text-slate-700">
              Guest Count
            </label>
            <input
              id="guestCount"
              type="number"
              min="1"
              value={form.guestCount}
              onChange={(event) => updateField('guestCount', event.target.value)}
              className={fieldClass(showValidation && validationErrors.guestCount)}
            />
            {showValidation && validationErrors.guestCount && (
              <p className="mt-1 text-sm text-red-600">{validationErrors.guestCount}</p>
            )}
          </div>

          <div className="flex flex-col justify-center gap-4">
            <label className="flex items-center gap-2 text-sm text-slate-700">
              <input
                type="checkbox"
                checked={form.needsParking}
                onChange={(event) => updateField('needsParking', event.target.checked)}
                className="h-4 w-4 rounded border-slate-300 text-sky-600 focus:ring-sky-500"
              />
              Need Parking
            </label>

            <label className="flex items-center gap-2 text-sm text-slate-700">
              <input
                type="checkbox"
                checked={form.isLateArrival}
                onChange={(event) =>
                  setForm((current) => ({
                    ...current,
                    isLateArrival: event.target.checked,
                    estimatedArrivalTime: event.target.checked ? current.estimatedArrivalTime : '',
                  }))
                }
                className="h-4 w-4 rounded border-slate-300 text-sky-600 focus:ring-sky-500"
              />
              Late Arrival
            </label>
          </div>

          {form.isLateArrival && (
            <div className="md:col-span-2">
              <label htmlFor="estimatedArrivalTime" className="block text-sm font-medium text-slate-700">
                Estimated Arrival Time
              </label>
              <input
                id="estimatedArrivalTime"
                type="datetime-local"
                value={form.estimatedArrivalTime}
                onChange={(event) => updateField('estimatedArrivalTime', event.target.value)}
                className={fieldClass(showValidation && validationErrors.estimatedArrivalTime)}
              />
              {showValidation && validationErrors.estimatedArrivalTime && (
                <p className="mt-1 text-sm text-red-600">{validationErrors.estimatedArrivalTime}</p>
              )}
            </div>
          )}
        </div>

        <div className="mt-8 flex items-center justify-end gap-3 border-t border-slate-200 pt-6">
          <Link
            to="/bookings"
            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Cancel
          </Link>
          <button
            type="submit"
            disabled={!isFormValid || submitting || loadingRooms}
            className="rounded-lg bg-sky-600 px-4 py-2 text-sm font-medium text-white hover:bg-sky-700 disabled:cursor-not-allowed disabled:bg-slate-300"
          >
            {submitting ? 'Creating...' : 'Create Booking'}
          </button>
        </div>
      </form>
    </section>
  )
}
