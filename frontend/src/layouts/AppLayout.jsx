import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

const navLinkClass = ({ isActive }) =>
  [
    'rounded-md px-3 py-2 text-sm font-medium transition-colors',
    isActive
      ? 'bg-sky-100 text-sky-800'
      : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900',
  ].join(' ')

export default function AppLayout() {
  const { isAuthenticated, user, loading, logout } = useAuth()

  const handleLogout = async () => {
    try {
      await logout()
    } catch {
      // Session is cleared locally even if the request fails.
    }
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <header className="border-b border-slate-200 bg-white shadow-sm">
        <div className="mx-auto flex max-w-5xl items-center justify-between gap-4 px-4 py-4">
          <div>
            <p className="text-lg font-semibold text-sky-800">SkagenBooking</p>
            <p className="text-sm text-slate-500">Pernille&apos;s Bed &amp; Breakfast</p>
          </div>
          <div className="flex flex-wrap items-center justify-end gap-2">
            <nav className="flex gap-2">
              <NavLink to="/" end className={navLinkClass}>
                Home
              </NavLink>
              <NavLink to="/bookings" className={navLinkClass}>
                Bookings
              </NavLink>
              {isAuthenticated && (
                <NavLink to="/bookings/new" className={navLinkClass}>
                  New Booking
                </NavLink>
              )}
            </nav>

            {!loading && (
              <div className="flex items-center gap-2 border-l border-slate-200 pl-2">
                {isAuthenticated ? (
                  <>
                    <span className="hidden text-sm text-slate-600 sm:inline">
                      {user?.email}
                    </span>
                    <button
                      type="button"
                      onClick={handleLogout}
                      className="rounded-md px-3 py-2 text-sm font-medium text-slate-600 hover:bg-slate-100 hover:text-slate-900"
                    >
                      Logout
                    </button>
                  </>
                ) : (
                  <>
                    <NavLink to="/login" className={navLinkClass}>
                      Login
                    </NavLink>
                    <NavLink to="/register" className={navLinkClass}>
                      Register
                    </NavLink>
                  </>
                )}
              </div>
            )}
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-5xl px-4 py-8">
        <Outlet />
      </main>
    </div>
  )
}
