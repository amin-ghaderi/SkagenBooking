import { useCallback, useEffect, useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import BookingTable from '../components/BookingTable'
import EmptyState from '../components/EmptyState'
import LoadingSpinner from '../components/LoadingSpinner'
import { deleteBooking, getBookings } from '../services/bookingsApi'

export default function BookingsPage() {
  const location = useLocation()
  const navigate = useNavigate()
  const { isAuthenticated } = useAuth()
  const [bookings, setBookings] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(false)
  const [successMessage, setSuccessMessage] = useState(location.state?.message ?? '')

  const loadBookings = useCallback(async () => {
    setLoading(true)
    setError(false)

    try {
      const data = await getBookings()
      setBookings(data)
    } catch {
      setError(true)
      setBookings([])
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    let active = true

    ;(async () => {
      try {
        const data = await getBookings()
        if (active) {
          setBookings(data)
          setError(false)
        }
      } catch {
        if (active) {
          setError(true)
          setBookings([])
        }
      } finally {
        if (active) {
          setLoading(false)
        }
      }
    })()

    return () => {
      active = false
    }
  }, [])

  const handleCancel = async (id) => {
    if (!isAuthenticated) {
      navigate('/login', { state: { from: location } })
      return
    }

    const confirmed = window.confirm(`Cancel booking #${id}?`)
    if (!confirmed) {
      return
    }

    try {
      await deleteBooking(id)
      await loadBookings()
    } catch {
      window.alert('Failed to cancel booking. Please try again.')
    }
  }

  return (
    <section className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-slate-900">Bookings</h1>
        {isAuthenticated ? (
          <Link
            to="/bookings/new"
            className="rounded-lg bg-sky-600 px-4 py-2 text-sm font-medium text-white hover:bg-sky-700"
          >
            New Booking
          </Link>
        ) : (
          <Link
            to="/login"
            className="rounded-lg bg-sky-600 px-4 py-2 text-sm font-medium text-white hover:bg-sky-700"
          >
            Login to book
          </Link>
        )}
      </div>

      {successMessage && (
        <div
          className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-800 shadow-sm"
          role="status"
        >
          <div className="flex items-start justify-between gap-3">
            <p>{successMessage}</p>
            <button
              type="button"
              onClick={() => setSuccessMessage('')}
              className="shrink-0 rounded-md px-2 py-1 text-xs font-medium text-emerald-700 hover:bg-emerald-100"
            >
              Dismiss
            </button>
          </div>
        </div>
      )}

      {loading && <LoadingSpinner />}

      {!loading && error && (
        <div className="rounded-xl border border-red-200 bg-white p-6 text-center shadow-sm">
          <p className="text-slate-900">Failed to load bookings.</p>
          <button
            type="button"
            onClick={loadBookings}
            className="mt-4 rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Retry
          </button>
        </div>
      )}

      {!loading && !error && bookings.length === 0 && <EmptyState />}

      {!loading && !error && bookings.length > 0 && (
        <BookingTable bookings={bookings} onCancel={handleCancel} />
      )}
    </section>
  )
}
