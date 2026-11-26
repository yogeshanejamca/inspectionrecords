import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { loadSettingsFromStorage } from './storage';

export interface SettingsState {
  theme: 'light' | 'dark';
  autoRefresh: boolean;
  autoRefreshInterval: number;
  itemsPerPage: number;
}

const defaultSettings: SettingsState = {
  theme: 'light',
  autoRefresh: true,
  autoRefreshInterval: 30000, // 30 seconds
  itemsPerPage: 10,
};

const initialState: SettingsState = {
  ...defaultSettings,
  ...loadSettingsFromStorage(),
};

export const settingsSlice = createSlice({
  name: 'settings',
  initialState,
  reducers: {
    setTheme: (state, action: PayloadAction<'light' | 'dark'>) => {
      state.theme = action.payload;
    },
    setAutoRefresh: (state, action: PayloadAction<boolean>) => {
      state.autoRefresh = action.payload;
    },
    setAutoRefreshInterval: (state, action: PayloadAction<number>) => {
      state.autoRefreshInterval = action.payload;
    },
    setItemsPerPage: (state, action: PayloadAction<number>) => {
      state.itemsPerPage = action.payload;
    },
    setSettings: (state, action: PayloadAction<Partial<SettingsState>>) => {
      return { ...state, ...action.payload };
    },
  },
});

export const {
  setTheme,
  setAutoRefresh,
  setAutoRefreshInterval,
  setItemsPerPage,
  setSettings,
} = settingsSlice.actions;

export default settingsSlice.reducer;
