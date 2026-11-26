import type { Inspection } from '../types/inspection';

const API_BASE = import.meta.env.VITE_API_URL ?? '';

async function getErrorMessage(response: Response): Promise<string> {
  const body = await response.text();
  if (!body) {
    return `Request failed (${response.status})`;
  }

  try {
    const error = JSON.parse(body) as {
      detail?: string;
      title?: string;
      message?: string;
    };
    return error.detail ?? error.title ?? error.message ?? body;
  } catch {
    return body;
  }
}

export async function createInspection(input: {
  templateId: number | null;
  title: string;
  description: string;
  scheduledAt: string;
}): Promise<string> {
  const response = await fetch(`${API_BASE}/api/Inspections`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(input),
  });

  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }

  let result: { id?: unknown; inspectionId?: unknown; recordId?: unknown };
  try {
    result = await response.json() as typeof result;
  } catch {
    throw new Error('Inspection was created, but the API response did not include a readable record ID.');
  }

  const id = result.inspectionId ?? result.recordId ?? result.id;
  if ((typeof id !== 'string' && typeof id !== 'number') || String(id).trim() === '') {
    throw new Error('Inspection was created, but the API response did not include a record ID.');
  }

  return String(id);
}

export async function uploadInspectionImage(
  recordId: string,
  image: File,
): Promise<void> {
  const formData = new FormData();
  formData.append('file', image, image.name);

  const response = await fetch(
    `${API_BASE}/api/inspection-records/${encodeURIComponent(recordId)}/image/upload`,
    { method: 'POST', body: formData },
  );

  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }
}

export async function fetchInspections(): Promise<Inspection[]> {
  const response = await fetch(`${API_BASE}/api/inspections`);

  if (!response.ok) {
    throw new Error(`Failed to load inspections (${response.status})`);
  }

  return response.json() as Promise<Inspection[]>;
}

export async function fetchInspection(id: number): Promise<Inspection> {
  const response = await fetch(`${API_BASE}/api/inspections/${id}`);
  if (!response.ok) throw new Error(await getErrorMessage(response));
  return response.json() as Promise<Inspection>;
}

export async function fetchInspectionImageNames(recordId: number): Promise<string[]> {
  const response = await fetch(
    `${API_BASE}/api/inspection-records/${encodeURIComponent(recordId)}/image`,
  );
  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }

  const result: unknown = await response.json();
  const imageRecords = Array.isArray(result) ? result : [result];
  const blobNames = imageRecords.flatMap((imageRecord) => {
    if (!imageRecord || typeof imageRecord !== 'object') return [];
    const blobName = (imageRecord as { blobName?: unknown }).blobName;
    return typeof blobName === 'string' && blobName.trim()
      ? [blobName]
      : [];
  });

  if (imageRecords.length > 0 && blobNames.length === 0) {
    throw new Error('The image metadata response did not contain a blobName.');
  }
  return blobNames;
}

export async function fetchInspectionImage(
  recordId: number,
  blobName: string,
): Promise<Blob> {
  const response = await fetch(
    `${API_BASE}/api/inspection-records/${encodeURIComponent(recordId)}/image/download?fileName=${encodeURIComponent(blobName)}`,
  );
  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }
  return response.blob();
}

export async function updateInspection(inspection: Inspection): Promise<Inspection> {
  const response = await fetch(`${API_BASE}/api/inspections/${inspection.id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(inspection),
  });
  if (!response.ok) throw new Error(await getErrorMessage(response));
  return response.json() as Promise<Inspection>;
}

export async function deleteInspection(id: number): Promise<void> {
  const response = await fetch(`${API_BASE}/api/inspections/${id}`, { method: 'DELETE' });
  if (!response.ok) throw new Error(await getErrorMessage(response));
}
