import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { deleteInspection, updateInspection } from '../api/inspectionsApi';
import type { Inspection } from '../types/inspection';

const statuses = ['Draft', 'Scheduled', 'InProgress', 'Completed', 'Cancelled', 'Paused'];

function localDateTime(value: string) {
  const date = new Date(value);
  return new Date(date.getTime() - date.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
}

export function InspectionEdit({
  inspection,
  onSave,
}: {
  inspection: Inspection;
  onSave: (item: Inspection) => void;
}) {
  const navigate = useNavigate();
  const [title, setTitle] = useState(inspection.title);
  const [description, setDescription] = useState(inspection.description ?? '');
  const [scheduledAt, setScheduledAt] = useState(localDateTime(inspection.scheduledAt));
  const [status, setStatus] = useState(inspection.status);
  const [busy, setBusy] = useState(false);
  const [message, setMessage] = useState('');

  async function save(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setMessage('');
    try {
      const saved = await updateInspection({
        ...inspection,
        title: title.trim(),
        description: description.trim(),
        status,
        scheduledAt: new Date(scheduledAt).toISOString(),
      });
      onSave(saved);
      setMessage('Changes saved.');
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Changes could not be saved.');
    } finally {
      setBusy(false);
    }
  }

  async function remove() {
    if (!window.confirm(`Delete "${inspection.title}"?`)) return;
    setBusy(true);
    setMessage('');
    try {
      await deleteInspection(inspection.id);
      navigate('/');
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Inspection could not be deleted.');
      setBusy(false);
    }
  }

  return (
    <form className="inspection-form" onSubmit={save}>
      <fieldset disabled={busy} className="d-grid gap-3">
        <legend>Edit inspection</legend>
        <label className="inspection-form__field">
          Title
          <input required maxLength={200} value={title} onChange={(event) => setTitle(event.target.value)} />
        </label>
        <label className="inspection-form__field">
          Description
          <textarea maxLength={2000} value={description} onChange={(event) => setDescription(event.target.value)} />
        </label>
        <label className="inspection-form__field">
          Scheduled date and time
          <input required type="datetime-local" value={scheduledAt} onChange={(event) => setScheduledAt(event.target.value)} />
        </label>
        <label className="inspection-form__field">
          Status
          <select value={status} onChange={(event) => setStatus(event.target.value)}>
            {statuses.map((option) => <option key={option}>{option}</option>)}
          </select>
        </label>
        <div className="d-flex gap-2">
          <button className="inspection-form__submit" type="submit">Save changes</button>
          <button className="btn btn-outline-danger" type="button" onClick={() => void remove()}>Delete inspection</button>
        </div>
        {message && <p role="status">{message}</p>}
      </fieldset>
    </form>
  );
}
