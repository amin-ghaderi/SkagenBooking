import { bookingFieldClass } from '../utils/bookingForm'

export default function BookingForm({
  form,
  properties,
  rooms,
  loadingRooms,
  showValidation,
  validationErrors,
  disabled = false,
  lockPropertyAndRoom = false,
  onSubmit,
  onFieldChange,
  onPropertyChange,
  onLateArrivalChange,
  actions,
}) {
  return (
    <form
      onSubmit={onSubmit}
      className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
      noValidate
    >
      <fieldset disabled={disabled} className="disabled:opacity-70">
        <div className="grid gap-6 md:grid-cols-2">
          <div>
            <label htmlFor="propertyId" className="block text-sm font-medium text-slate-700">
              Property
            </label>
            <select
              id="propertyId"
              value={form.propertyId}
              onChange={(event) => onPropertyChange(event.target.value)}
              disabled={lockPropertyAndRoom}
              className={bookingFieldClass(showValidation && validationErrors.propertyId)}
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
              onChange={(event) => onFieldChange('roomId', event.target.value)}
              disabled={lockPropertyAndRoom || !form.propertyId || loadingRooms}
              className={bookingFieldClass(showValidation && validationErrors.roomId)}
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
              onChange={(event) => onFieldChange('checkInDate', event.target.value)}
              className={bookingFieldClass(showValidation && validationErrors.checkInDate)}
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
              onChange={(event) => onFieldChange('checkOutDate', event.target.value)}
              className={bookingFieldClass(showValidation && validationErrors.checkOutDate)}
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
              onChange={(event) => onFieldChange('guestCount', event.target.value)}
              className={bookingFieldClass(showValidation && validationErrors.guestCount)}
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
                onChange={(event) => onFieldChange('needsParking', event.target.checked)}
                className="h-4 w-4 rounded border-slate-300 text-sky-600 focus:ring-sky-500"
              />
              Need Parking
            </label>

            <label className="flex items-center gap-2 text-sm text-slate-700">
              <input
                type="checkbox"
                checked={form.isLateArrival}
                onChange={(event) => onLateArrivalChange(event.target.checked)}
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
                onChange={(event) => onFieldChange('estimatedArrivalTime', event.target.value)}
                className={bookingFieldClass(showValidation && validationErrors.estimatedArrivalTime)}
              />
              {showValidation && validationErrors.estimatedArrivalTime && (
                <p className="mt-1 text-sm text-red-600">{validationErrors.estimatedArrivalTime}</p>
              )}
            </div>
          )}
        </div>
      </fieldset>

      <div className="mt-8 flex items-center justify-end gap-3 border-t border-slate-200 pt-6">
        {actions}
      </div>
    </form>
  )
}
