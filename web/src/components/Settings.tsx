import { useState } from 'react';
import {
  setTheme,
  setAutoRefresh,
  setAutoRefreshInterval,
  setItemsPerPage,
} from '../store/settingsSlice';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import './Settings.css';

export function Settings() {
  const dispatch = useAppDispatch();
  const settings = useAppSelector((state) => state.settings);
  const [isOpen, setIsOpen] = useState(false);

  const handleThemeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    dispatch(setTheme(e.target.value as 'light' | 'dark'));
  };

  const handleAutoRefreshChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    dispatch(setAutoRefresh(e.target.checked));
  };

  const handleIntervalChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(e.target.value, 10);
    if (!isNaN(value) && value > 0) {
      dispatch(setAutoRefreshInterval(value));
    }
  };

  const handleItemsPerPageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(e.target.value, 10);
    if (!isNaN(value) && value > 0) {
      dispatch(setItemsPerPage(value));
    }
  };

  return (
    <div className="settings-container">
      <button
        className="settings-toggle"
        onClick={() => setIsOpen(!isOpen)}
        aria-expanded={isOpen}
        title="Settings"
      >
        ⚙️
      </button>

      {isOpen && (
        <div className="settings-panel">
          <div className="settings-header">
            <h2>Settings</h2>
            <button
              className="settings-close"
              onClick={() => setIsOpen(false)}
              aria-label="Close settings"
            >
              ✕
            </button>
          </div>

          <div className="settings-content">
            <div className="setting-group">
              <label htmlFor="theme-select">Theme:</label>
              <select
                id="theme-select"
                value={settings.theme}
                onChange={handleThemeChange}
              >
                <option value="light">Light</option>
                <option value="dark">Dark</option>
              </select>
            </div>

            <div className="setting-group">
              <label htmlFor="auto-refresh">
                <input
                  id="auto-refresh"
                  type="checkbox"
                  checked={settings.autoRefresh}
                  onChange={handleAutoRefreshChange}
                />
                Enable Auto-Refresh
              </label>
            </div>

            {settings.autoRefresh && (
              <div className="setting-group">
                <label htmlFor="refresh-interval">
                  Refresh Interval (ms):
                </label>
                <input
                  id="refresh-interval"
                  type="number"
                  min="5000"
                  step="1000"
                  value={settings.autoRefreshInterval}
                  onChange={handleIntervalChange}
                />
              </div>
            )}

            <div className="setting-group">
              <label htmlFor="items-per-page">Items Per Page:</label>
              <input
                id="items-per-page"
                type="number"
                min="5"
                max="100"
                step="5"
                value={settings.itemsPerPage}
                onChange={handleItemsPerPageChange}
              />
            </div>


          </div>
        </div>
      )}
    </div>
  );
}
