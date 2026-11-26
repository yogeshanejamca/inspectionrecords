import { configureStore, createListenerMiddleware } from '@reduxjs/toolkit';
import settingsReducer from './settingsSlice';
import { saveSettingsToStorage } from './storage';
import stateReducer from './stateSlice';

const listenerMiddleware = createListenerMiddleware();

export const store = configureStore({
  reducer: {
    settings: settingsReducer,
    states: stateReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().prepend(listenerMiddleware.middleware),
});

listenerMiddleware.startListening({
  predicate: (action) => action.type.startsWith('settings/'),
  effect: (_, api) => {
    const state = api.getState() as RootState;
    saveSettingsToStorage(state.settings);
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
