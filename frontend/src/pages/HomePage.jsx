import { Link } from 'react-router-dom'

export default function HomePage() {
  return (
    <section className="rounded-xl border border-slate-200 bg-white p-8 shadow-sm">
      <h1 className="text-3xl font-bold text-slate-900">Welcome to SkagenBooking</h1>
      <p className="mt-3 max-w-2xl text-slate-600">
        Frontend foundation for managing room bookings. Business features will be
        added in upcoming phases.
      </p>
      <div className="mt-6 flex gap-3">
        <Link
          to="/bookings"
          className="rounded-lg bg-sky-600 px-4 py-2 text-sm font-medium text-white hover:bg-sky-700"
        >
          View Bookings
        </Link>
        <Link
          to="/bookings/new"
          className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        >
          Create Booking
        </Link>
      </div>
    </section>
  )
}
