import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import {
  fetchInspection,
  fetchInspectionImage,
  fetchInspectionImageNames,
} from '../api/inspectionsApi';
import type { Inspection } from '../types/inspection';
import { InspectionEdit } from './InspectionEdit';

function formatDate(value: string): string {
  return new Date(value).toLocaleString(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short',
  });
}

export function InspectionDetails() {
  const { inspectionId } = useParams<{ inspectionId: string }>();
  const [inspection, setInspection] = useState<Inspection | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [imageUrls, setImageUrls] = useState<string[]>([]);
  const [imageError, setImageError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;
    const objectUrls: string[] = [];

    async function loadInspection() {
      const id = Number(inspectionId);
      if (!Number.isInteger(id) || id <= 0) {
        setError('The inspection ID is invalid.');
        setLoading(false);
        return;
      }

      setLoading(true);
      setError(null);
      setInspection(null);
      setImageUrls([]);
      setImageError(null);

      try {
        const item = await fetchInspection(id);
        if (!active) return;
        setInspection(item);

        try {
          const blobNames = await fetchInspectionImageNames(id);
          if (!active) return;
          if (blobNames.length === 0) {
            setImageError('No images are available for this inspection.');
          } else {
            const blobs = await Promise.all(
              blobNames.map((blobName) => fetchInspectionImage(id, blobName)),
            );
            if (!active) return;
            blobs.forEach((blob) => objectUrls.push(URL.createObjectURL(blob)));
            setImageUrls([...objectUrls]);
          }
        } catch (imageLoadError) {
          if (!active) return;
          setImageError(
            imageLoadError instanceof Error
              ? imageLoadError.message
              : 'Could not load the inspection images.',
          );
        }
      } catch (loadError) {
        if (!active) return;
        setError(
          loadError instanceof Error
            ? loadError.message
            : 'Could not load inspection details.',
        );
      } finally {
        if (active) setLoading(false);
      }
    }

    void loadInspection();
    return () => {
      active = false;
      objectUrls.forEach((objectUrl) => URL.revokeObjectURL(objectUrl));
    };
  }, [inspectionId]);

  return (
    <section className="inspection-detail-page">
      <Link to="/" className="inspection-form-page__back">Back to inspections</Link>
      {loading && <p className="loading" role="status">Loading inspection…</p>}
      {error && <div className="error-banner" role="alert">{error}</div>}
      {!loading && inspection && (
        <article className="inspection-detail">
          <div className="inspection-detail__header">
            <div>
              <p className="eyebrow">Inspection #{inspection.id}</p>
              <h1>{inspection.title}</h1>
            </div>
            <span className="status-badge">{inspection.status}</span>
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
            {inspection.updatedAt && (
              <div>
                <dt>Updated</dt>
                <dd>{formatDate(inspection.updatedAt)}</dd>
              </div>
            )}
          </dl>
          <InspectionEdit key={inspection.id} inspection={inspection} onSave={setInspection} />
          <section className="inspection-detail__image-section">
            <h2>Inspection image</h2>
            {imageUrls.map((imageUrl, index) => (
              <img
                key={imageUrl}
                className="inspection-detail__image"
                src={imageUrl}
                alt={`Image ${index + 1} for ${inspection.title}`}
              />
            ))}
            {imageError && <p className="inspection-detail__image-message">{imageError}</p>}
          </section>
        </article>
      )}
    </section>
  );
}
