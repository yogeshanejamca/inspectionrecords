import { useEffect, useState } from 'react';
import { Link, Route, Routes } from 'react-router-dom';
import { fetchInspections } from './api/inspectionsApi';
import { InspectionAdd } from './components/InspectionAdd';
import { InspectionDetails } from './components/InspectionDetails';
import { InspectionList } from './components/InspectionList';
import { RecordAdd } from './components/Records/RecordAdd';
import { StateRecordList } from './components/Records/RecordList';
import { Settings } from './components/Settings';
import AppInitializer from './helpers/AppInitializer';
import { useAppSelector } from './store/hooks';
import type { Inspection } from './types/inspection';
import './App.css';

function InspectionHome() {
  const [inspections, setInspections] = useState<Inspection[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  const { autoRefresh, autoRefreshInterval, itemsPerPage } = useAppSelector((state) => state.settings);
  const [page, setPage] = useState(1);
  const pageSize = Math.max(1, itemsPerPage);
  const pageCount = Math.max(1, Math.ceil((inspections?.length ?? 0) / pageSize));
  const currentPage = Math.min(page, pageCount);

  useEffect(() => {
    let active = true;
    async function load() {
      try {
        const items = await fetchInspections();
        if (active) {
          setInspections(items);
          setError(null);
        }
      } catch (failure) {
        if (active) setError(failure instanceof Error ? failure.message : 'Could not load inspections.');
      }
    }
    void load();
    const timer = autoRefresh ? window.setInterval(load, autoRefreshInterval) : undefined;
    return () => {
      active = false;
      window.clearInterval(timer);
    };
  }, [autoRefresh, autoRefreshInterval]);

  return (
    <section className="app">
      <header className="app-header">
        <h1>Inspections</h1>
        <Settings />
      </header>
      {error && <p className="error-banner" role="alert">{error}</p>}
      {inspections && (
        <InspectionList inspections={inspections.slice((currentPage - 1) * pageSize, currentPage * pageSize)} />
      )}
      {pageCount > 1 && (
        <nav className="d-flex align-items-center gap-2" aria-label="Inspection pages">
          <button type="button" disabled={currentPage === 1} onClick={() => setPage(currentPage - 1)}>Previous</button>
          <span>Page {currentPage} of {pageCount}</span>
          <button type="button" disabled={currentPage === pageCount} onClick={() => setPage(currentPage + 1)}>Next</button>
        </nav>
      )}
      {!inspections && !error && <p role="status">Loading inspections…</p>}
    </section>
  );
}

export default function App() {
  const theme = useAppSelector((state) => state.settings.theme);
  useEffect(() => {
    document.documentElement.dataset.theme = theme;
    document.documentElement.dataset.bsTheme = theme;
  }, [theme]);
  return (
    <>
      <AppInitializer />
      <nav className="app-nav" aria-label="Main navigation">
        <Link className="app-nav__link" to="/">Inspections</Link>
        <Link className="app-nav__link" to="/inspections/add">Add inspection</Link>
        <Link className="app-nav__link" to="/records">State records</Link>
      </nav>
      <main>
        <Routes>
          <Route path="/" element={<InspectionHome />} />
          <Route path="/inspections/add" element={<InspectionAdd />} />
          <Route path="/inspections/:inspectionId" element={<InspectionDetails />} />
          <Route path="/records" element={<StateRecordList />} />
          <Route path="/records/add" element={<RecordAdd />} />
          <Route path="*" element={<p>Page unavailable. <Link to="/">Back to inspections</Link></p>} />
        </Routes>
      </main>
    </>
  );
}
