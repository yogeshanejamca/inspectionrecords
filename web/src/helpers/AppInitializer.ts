import { useEffect } from "react";
import { fetchStates } from "../store/stateSlice";
import { useAppDispatch } from "../store/hooks";

function AppInitializer() {
    const dispatch = useAppDispatch();

    useEffect(() => {
        dispatch(fetchStates());
    }, [dispatch]);

    return null;
}
export default AppInitializer;