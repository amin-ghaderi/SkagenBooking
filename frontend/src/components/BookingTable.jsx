import { Link } from 'react-router-dom'

function formatDate(value) {
  return new Date(value).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

function StatusBadge({ status }) {
  const styles = {
    Pending: 'bg-amber-100 text-amber-800',
    Confirmed: 'bg-emerald-100 text-emerald-800',
    Cancelled: 'bg-slate-100 text-slate-600',
  }

  const className = styles[status] ?? 'bg-slate-100 text-slate-600'

  return (
    <span className={`inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium ${className}`}>
      {status}
    </span>
  )
}

export default function BookingTable({ bookings, onCancel }) {
  return (
    <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white shadow-sm">
      <table className="min-w-full divide-y divide-slate-200 text-left text-sm">
        <thead className="bg-slate-50">
          <tr>
            <th className="px-4 py-3 font-medium text-slate-600">ID</th>
            <th className="px-4 py-3 font-medium text-slate-600">Property</th>
            <th className="px-4 py-3 font-medium text-slate-600">Room</th>
            <th className="px-4 py-3 font-medium text-slate-600">Check In</th>
            <th className="px-4 py-3 font-medium text-slate-600">Check Out</th>
            <th className="px-4 py-3 font-medium text-slate-600">Guests</th>
            <th className="px-4 py-3 font-medium text-slate-600">Status</th>
            <th className="px-4 py-3 font-medium text-slate-600">Parking</th>
            <th className="px-4 py-3 font-medium text-slate-600">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-200">
          {bookings.map((booking) => {
            const isCancelled = booking.status === 'Cancelled'

            return (
              <tr key={booking.id} className="text-slate-900">
                <td className="px-4 py-3 font-mono text-slate-700">#{booking.id}</td>
                <td className="px-4 py-3">Property {booking.propertyId}</td>
                <td className="px-4 py-3">Room {booking.roomId}</td>
                <td className="px-4 py-3">{formatDate(booking.checkInDate)}</td>
                <td className="px-4 py-3">{formatDate(booking.checkOutDate)}</td>
                <td className="px-4 py-3">{booking.guestCount}</td>
                <td className="px-4 py-3">
                  <StatusBadge status={booking.status} />
                </td>
                <td className="px-4 py-3">{booking.needsParking ? 'Yes' : 'No'}</td>
                <td className="px-4 py-3">
                  <div className="flex gap-2">
                    <Link
                      to={`/bookings/${booking.id}/edit`}
                      className="rounded-md border border-slate-300 px-2.5 py-1 text-xs font-medium text-slate-700 hover:bg-slate-50"
                    >
                      Edit
                    </Link>
                    <button
                      type="button"
                      onClick={() => onCancel(booking.id)}
                      disabled={isCancelled}
                      className="rounded-md border border-red-200 px-2.5 py-1 text-xs font-medium text-red-700 hover:bg-red-50 disabled:cursor-not-allowed disabled:border-slate-200 disabled:text-slate-400 disabled:hover:bg-transparent"
                    >
                      Cancel
                    </button>
                  </div>
                </td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
