import React, {useEffect} from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { getLookupRequest, valuesListSelector } from '../../ducks/lookup';

const SelectValue = ({ value, source, indexRow }) => {
    const dispatch = useDispatch();

    let lookup = useSelector(state => valuesListSelector(state, source)) || [];

    useEffect(() => {
        console.log('index', indexRow);
        if (!lookup.length && indexRow === 0) {
            dispatch(
                getLookupRequest({
                    name: source,
                    isForm: true,
                }),
            );
        }
    }, []);

    console.log('lookup', lookup, value);

    const valueText = lookup.find(x => x.value === value);

    return <>{valueText && valueText.name}</>;
};

export default SelectValue;
