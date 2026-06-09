export default function ErrorAlert({ message, onDismiss }) {
  if (!message) {
    return null
  }

  return (
    <div
      className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800 shadow-sm"
      role="alert"
    >
      <div className="flex items-start justify-between gap-3">
        <p>{message}</p>
        {onDismiss && (
          <button
            type="button"
            onClick={onDismiss}
            className="shrink-0 rounded-md px-2 py-1 text-xs font-medium text-red-700 hover:bg-red-100"
          >
            Dismiss
          </button>
        )}
      </div>
    </div>
  )
}
