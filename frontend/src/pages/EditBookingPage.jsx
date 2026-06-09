import { useParams } from 'react-router-dom'

export default function EditBookingPage() {
  const { id } = useParams()

  return (
    <section className="rounded-xl border border-dashed border-slate-300 bg-white p-8">
      <h1 className="text-2xl font-semibold text-slate-900">Edit Booking</h1>
      <p className="mt-2 text-slate-500">
        Placeholder — edit form for booking <span className="font-mono text-sky-700">#{id}</span> will be
        implemented in a later phase.
      </p>
    </section>
  )
}
