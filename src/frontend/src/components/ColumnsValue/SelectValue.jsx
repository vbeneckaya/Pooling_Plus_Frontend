import React, {useEffect} from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { getLookupRequest, valuesListSelector } from '../../ducks/lookup';

const SelectValue = ({ value, source, indexRow, showRawValue }) => {
    const dispatch = useDispatch();

    let lookup = useSelector(state => valuesListSelector(state, source)) || [];

    useEffect(() => {
        if (!lookup.length && indexRow === 0 && !showRawValue) {
            dispatch(
                getLookupRequest({
                    name: source,
                    isForm: true,
                }),
            );
        }
    }, []);

    const valueText = lookup.find(x => x.value === value);

    return <>{showRawValue ? value : valueText && valueText.name}</>;
};

export default SelectValue;
