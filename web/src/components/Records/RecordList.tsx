import { useCallback, useEffect, useState } from "react";
import { StateRecord } from "../../types/record";
import { Link } from "react-router-dom";


//Create API VARIABLE
const API_URL = import.meta.env.VITE_API_URL ?? '';

export async function fetchRecords(): Promise<StateRecord[]> {
    const response = await fetch(`${API_URL}/api/StateRecord/states`);

    if(!response.ok){

        throw new Error(`Failed to load records (${response.status})`);
    }

    const json = await response.json();
return Array.isArray(json) ? json : Object.values(json);
}

//Create card componennt for each record.

interface StateRecordCardProps {
    record: StateRecord;
}

export function StateRecordCard({record}:StateRecordCardProps){


    return(
        <article className="inspection-card">
            <div className="inspection-card__header">
            <h2>{record.name}</h2>
            <p>Emoji: {record.emoji}</p>
            <p>Walking Distance: {record.walkingDistance}</p>
            <p>Total Points: {record.totalPoints}</p>
            <div className="inspection-card__meta">
                {record.places.map(place => (
                    <div key={place.id}>
                        <h3>{place.name}</h3>
                        <p>{place.description}</p>
                        <p>Distance: {place.distance}</p>
                        <p>Points: {place.points}</p>
                        <p>Visited: {place.visited ? 'Yes' : 'No'}</p>
                    </div>
                ))}
            </div>
            </div>
        </article>
    )

}


//Create interface for
export function StateRecordList() {
    //Declare state for loading error and state records
    const [states,setStates] = useState<StateRecord[]>([]);
    const [loading,setLoading] = useState<boolean>(true);
    const [error,setError]= useState<string | null>(null);

    //Fetch records from API
    const loadStates = useCallback(async () => {
        setLoading(true);
        setError(null);
        try{
            const data = await fetchRecords();
            setStates(data);
        }catch(err){
            setError((err as Error).message);
        }finally{
            setLoading(false);
        }


    }, []);

    useEffect(()=>{
        loadStates();
    },[loadStates]);

    // if still loading show loading message
    // if error show error message
    if(loading){
        return <div>Loading records...</div>
    }
    if(error){
        return <div>Error loading records: {error}</div>
    }

    return(

        <div className="state-record-list">
                <Link to="/records/add">
                <button>+ Add Record</button>
                </Link>
            {states.map(state => (
                <StateRecordCard key={state.id} record={state} />
            ))}
        </div>
    )
}