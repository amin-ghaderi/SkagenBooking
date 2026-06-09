import { useEffect, useMemo, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import BookingForm from '../components/BookingForm'
import ErrorAlert from '../components/ErrorAlert'
import LoadingSpinner from '../components/LoadingSpinner'
import { parseApiError } from '../services/apiErrors'
import { getBookingById, updateBooking } from '../services/bookingsApi'
import { getProperties } from '../services/propertiesApi'
import { getRooms } from '../services/roomsApi'
import {
  bookingToForm,
  buildUpdatePayload,
  initialBookingForm,
  validateBookingForm,
} from '../utils/bookingForm'

export default function EditBookingPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [form, setForm] = useState(initialBookingForm)
  const [bookingStatus, setBookingStatus] = useState('')
  const [properties, setProperties] = useState([])
  const [rooms, setRooms] = useState([])
  const [loadingPage, setLoadingPage] = useState(true)
  const [loadingRooms, setLoadingRooms] = useState(false)
  const [notFound, setNotFound] = useState(false)
  const [loadError, setLoadError] = useState('')
  const [submitError, setSubmitError] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [showValidation, setShowValidation] = useState(false)

  const isCancelled = bookingStatus === 'Cancelled'

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
        const [booking, propertiesData] = await Promise.all([
          getBookingById(id),
          getProperties(),
        ])

        if (!active) {
          return
        }

        setBookingStatus(booking.status)
        setProperties(propertiesData)

        setLoadingRooms(true)
        const roomsData = await getRooms(booking.propertyId)

        if (!active) {
          return
        }

        setRooms(roomsData)
        setForm(bookingToForm(booking))
        setLoadError('')
      } catch (error) {
        if (!active) {
          return
        }

        if (error.response?.status === 404) {
          setNotFound(true)
        } else {
          setLoadError(parseApiError(error, 'Failed to load booking.'))
        }
      } finally {
        if (active) {
          setLoadingPage(false)
          setLoadingRooms(false)
        }
      }
    })()

    return () => {
      active = false
    }
  }, [id])

  const updateField = (name, value) => {
    setSubmitError('')
    setForm((current) => ({ ...current, [name]: value }))
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

    if (!isFormValid || isCancelled) {
      return
    }

    setSubmitting(true)

    try {
      await updateBooking(id, buildUpdatePayload(form))
      navigate('/bookings', { state: { message: 'Booking updated successfully.' } })
    } catch (error) {
      const status = error.response?.status
      let fallback = 'Failed to update booking. Please check the form and try again.'

      if (status === 404) {
        fallback = 'Booking not found.'
      } else if (status === 409) {
        fallback = 'Booking could not be updated due to a conflict.'
      }

      setSubmitError(parseApiError(error, fallback))
    } finally {
      setSubmitting(false)
    }
  }

  if (loadingPage) {
    return <LoadingSpinner />
  }

  if (notFound) {
    return (
      <section className="space-y-6">
        <div className="rounded-xl border border-slate-200 bg-white p-8 text-center shadow-sm">
          <h1 className="text-2xl font-semibold text-slate-900">Booking not found.</h1>
          <Link
            to="/bookings"
            className="mt-4 inline-block rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Back to Bookings
          </Link>
        </div>
      </section>
    )
  }

  return (
    <section className="space-y-6">
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-2xl font-semibold text-slate-900">Edit Booking</h1>
          <p className="mt-1 text-slate-500">
            Update reservation <span className="font-mono text-sky-700">#{id}</span>
          </p>
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
      {isCancelled && (
        <ErrorAlert message="This booking is cancelled and cannot be updated." />
      )}

      <BookingForm
        form={form}
        properties={properties}
        rooms={rooms}
        loadingRooms={loadingRooms}
        showValidation={showValidation}
        validationErrors={validationErrors}
        disabled={submitting || isCancelled}
        lockPropertyAndRoom
        onSubmit={handleSubmit}
        onFieldChange={updateField}
        onPropertyChange={() => {}}
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
              disabled={!isFormValid || submitting || loadingRooms || isCancelled}
              className="rounded-lg bg-sky-600 px-4 py-2 text-sm font-medium text-white hover:bg-sky-700 disabled:cursor-not-allowed disabled:bg-slate-300"
            >
              {submitting ? 'Updating...' : 'Update Booking'}
            </button>
          </>
        }
      />
    </section>
  )
}
