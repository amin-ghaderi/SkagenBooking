export default function EmptyState({ message = 'No bookings found.' }) {
  return (
    <div className="rounded-xl border border-dashed border-slate-300 bg-white px-6 py-12 text-center shadow-sm">
      <p className="text-slate-500">{message}</p>
    </div>
  )
}
