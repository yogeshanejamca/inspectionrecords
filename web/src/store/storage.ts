import type { SettingsState } from './settingsSlice';

const SETTINGS_STORAGE_KEY = 'inspections-app-settings';

export function loadSettingsFromStorage(): Partial<SettingsState> | null {
  try {
    const stored = localStorage.getItem(SETTINGS_STORAGE_KEY);
    if (stored) {
      return JSON.parse(stored) as Partial<SettingsState>;
    }
  } catch (error) {
    console.error('Failed to load settings from storage:', error);
  }
  return null;
}

export function saveSettingsToStorage(settings: SettingsState): void {
  try {
    localStorage.setItem(SETTINGS_STORAGE_KEY, JSON.stringify(settings));
  } catch (error) {
    console.error('Failed to save settings to storage:', error);
  }
}
