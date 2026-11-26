import type { Inspection } from '../types/inspection';
import { InspectionCard } from './InspectionCard';

interface InspectionListProps {
  inspections: Inspection[];
}

export function InspectionList({ inspections }: InspectionListProps) {
  if (inspections.length === 0) {
    return (
      <p className="empty-state">
        No inspections found. Add an inspection to get started.
      </p>
    );
  }

  return (
    <div className="inspection-grid">
      {inspections.map((inspection) => (
        <InspectionCard
          key={inspection.id}
          inspection={inspection}
        />
      ))}
    </div>
  );
}
