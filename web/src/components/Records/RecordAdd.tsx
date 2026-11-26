import { useMemo, useState } from "react"
import { places} from "../../types/record"
import { RootState } from "../../store/store";
import { useSelector } from "react-redux";

type ModalState = { show: boolean; success: boolean; message: string };

export function RecordAdd() {

    //Create state for the form data dropdown to select the stateId from the list of states.
   // const [states,setStates] = useState<StateRecord[]>([]);
    const [modal, setModal] = useState<ModalState>({ show: false, success: false, message: "" });
    const states = useSelector((state: RootState) => state.states);
    // Add this useMemo hook before the return statement
    const stateOptions = useMemo(
        () => states.map((s) => (
            <option key={s.id} value={s.id}>
                {s.emoji} {s.name}
            </option>
        )),
        [states]
    );



    const API_BASE = import.meta.env.VITE_API_URL ?? '';

const emptyForm: places = { id: '', name: '', icon: '', visited: false, description: '', distance: 0, points: 0, stateId: '' };

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        try {
            const response = await fetch(`${API_BASE}/api/StateRecord`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(form),
            });
            if (!response.ok) throw new Error(`Server error: ${response.status}`);
            await response.json();
            setModal({ show: true, success: true, message: "Place saved successfully!" });
            setForm(emptyForm);
        } catch (error) {
            console.error("Error saving place:", error);
            setModal({ show: true, success: false, message: "Failed to save. Please try again." });
        }
    }

const [form,setForm] = useState<places>({
    id:'',
    name:"",
    icon:"",
    visited:false,
    description:"",
    distance:0,
    points:0,
    stateId:""
})

function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    const { name, value, type, valueAsNumber } = e.target;
    setForm((prevForm) => ({
        ...prevForm,
        [name]: type === "number" ? valueAsNumber : value,
    }));
}

return (
    <>
    <form method="post" onSubmit={handleSubmit}>
         <div className="inspection-card__header">
             <div className="inspection-card__meta">
        <label className="inspection-card">Name</label>
        <input type="text" name="name" required value={form.name} onChange={handleChange} />
        </div>
           <div className="inspection-card__meta">
        <label className="inspection-card">Description</label>
        <input type="text" name="description" required value={form.description} onChange={handleChange} />
        </div>
         <div className="inspection-card__meta">
        <label className="inspection-card">Distance</label>
        <input type="number" name="distance" required step="any" min="0" value={form.distance} onChange={handleChange} />
        </div>
        <label className="inspection-card">Points</label>
        <input type="number" name="points" required min="0" value={form.points} onChange={handleChange} />
        <label className="inspection-card">Visited</label>
        <input type="checkbox" name="visited" checked={form.visited} onChange={(e) => setForm((prevForm) => ({
            ...prevForm,
            visited: e.target.checked
        }))} />
        <label className="inspection-card">State</label>
<select
    name="stateId" required
    value={form.stateId}
    onChange={(e) => setForm((prevForm) => ({ ...prevForm, stateId: e.target.value }))}
>
    <option value="">-- Select a State --</option>
    {stateOptions}{/* Use the memoized value here instead of mapping inline */}
</select>
        <label className="inspection-card">Icon</label>
        <input type="text" name="icon" required value={form.icon} onChange={handleChange} />
        <button type="submit">Save Place</button>
    </div>
    </form>

    {/* Bootstrap Modal */}
    {modal.show && (
        <>
            <div className="modal fade show d-block" tabIndex={-1} role="dialog">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className={`modal-header ${modal.success ? "bg-success" : "bg-danger"} text-white`}>
                            <h5 className="modal-title">{modal.success ? "Success" : "Error"}</h5>
                            <button type="button" className="btn-close btn-close-white" onClick={() => setModal(m => ({ ...m, show: false }))} />
                        </div>
                        <div className="modal-body">
                            <p>{modal.message}</p>
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-secondary" onClick={() => setModal(m => ({ ...m, show: false }))}>Close</button>
                        </div>
                    </div>
                </div>
            </div>
            <div className="modal-backdrop fade show" onClick={() => setModal(m => ({ ...m, show: false }))} />
        </>
    )}
    </>
)
}