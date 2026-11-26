export interface Inspection {
  id: number;
  title: string;
  description: string | null;
  status: string;
  scheduledAt: string;
  createdAt: string;
  updatedAt: string | null;
}
