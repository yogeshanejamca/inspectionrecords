import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { StateRecord } from '../types/record';

// recordsSlice.ts

const initialState: StateRecord[] = [];
const API_BASE = import.meta.env.VITE_API_URL ?? '';

export const fetchStates = createAsyncThunk(
    'states/fetchStates',
    async (): Promise<StateRecord[]> => {
        const response = await fetch(`${API_BASE}/api/StateRecord/states`);
        const data = await response.json();
        return Array.isArray(data) ? data : Object.values(data);
    }
);

export const statesSlice = createSlice({
    name: 'states',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder.addCase(fetchStates.fulfilled, (_state, action) => {
            return action.payload;
        });
    }
});

export default statesSlice.reducer;
