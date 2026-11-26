import type { Inspection } from '../types/inspection';
import { useNavigate } from 'react-router-dom';

const statusStyles: Record<string, string> = {
  Draft: 'status-draft',
  Scheduled: 'status-scheduled',
  InProgress: 'status-progress',
  Completed: 'status-completed',
  Cancelled: 'status-cancelled',
};

interface InspectionCardProps {
  inspection: Inspection;
}

function formatDate(value: string) {
  return new Date(value).toLocaleString(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short',
  });
}

export function InspectionCard({ inspection }: InspectionCardProps) {
  const statusClass = statusStyles[inspection.status] ?? 'status-draft';
  const navigate = useNavigate();

  return (
    <article className="inspection-card">
      <div className="inspection-card__header">
        <h2>
          <button
            type="button"
            className="inspection-card__title-button"
            onClick={() => navigate(`/inspections/${inspection.id}`)}
          >
            {inspection.title}
          </button>
        </h2>
        <span className={`status-badge ${statusClass}`}>{inspection.status}</span>
      </div>
      {inspection.description && (
        <p className="inspection-card__description">{inspection.description}</p>
      )}
      <dl className="inspection-card__meta">
        <div>
          <dt>Scheduled</dt>
          <dd>{formatDate(inspection.scheduledAt)}</dd>
        </div>
        <div>
          <dt>Created</dt>
          <dd>{formatDate(inspection.createdAt)}</dd>
        </div>
      </dl>
    </article>
  );
}
