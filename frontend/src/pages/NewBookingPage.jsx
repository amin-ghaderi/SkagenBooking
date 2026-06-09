import { useEffect, useMemo, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import BookingForm from '../components/BookingForm'
import ErrorAlert from '../components/ErrorAlert'
import LoadingSpinner from '../components/LoadingSpinner'
import { parseApiError } from '../services/apiErrors'
import { createBooking } from '../services/bookingsApi'
import { getProperties } from '../services/propertiesApi'
import { getRooms } from '../services/roomsApi'
import {
  buildCreatePayload,
  initialBookingForm,
  validateBookingForm,
} from '../utils/bookingForm'

export default function NewBookingPage() {
  const navigate = useNavigate()
  const [form, setForm] = useState(initialBookingForm)
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

  const validationErrors = useMemo(() => validateBookingForm(form, selectedRoom), [form, selectedRoom])
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

  const handleLateArrivalChange = (isLateArrival) => {
    setSubmitError('')
    setForm((current) => ({
      ...current,
      isLateArrival,
      estimatedArrivalTime: isLateArrival ? current.estimatedArrivalTime : '',
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
      await createBooking(buildCreatePayload(form))
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

      <BookingForm
        form={form}
        properties={properties}
        rooms={rooms}
        loadingRooms={loadingRooms}
        showValidation={showValidation}
        validationErrors={validationErrors}
        disabled={submitting}
        onSubmit={handleSubmit}
        onFieldChange={updateField}
        onPropertyChange={handlePropertyChange}
        onLateArrivalChange={handleLateArrivalChange}
        actions={
          <>
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
          </>
        }
      />
    </section>
  )
}
