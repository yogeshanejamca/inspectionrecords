import type { InspectionTemplate } from '../types/InspectionTemplate';

const API_BASE = import.meta.env.VITE_API_URL ?? '';



export async function fetchTemplates(): Promise<InspectionTemplate[]> {
  const response = await fetch(`${API_BASE}/api/Template`);
  if (!response.ok) {
    throw new Error(`Failed to load templates (${response.status})`);
  }

  return response.json() as Promise<InspectionTemplate[]>;
}
