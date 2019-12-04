import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { getLookupRequest, valuesListSelector } from '../../ducks/lookup';
import TextCropping from './TextCropping';

const SelectValue = ({value, source, indexRow, indexColumn, showRawValue, width}) => {
    /*const dispatch = useDispatch();

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

    const valueText = lookup && lookup.length && lookup.find(x => x.value === value);
*/
    console.log('selectCell', value, showRawValue);
    return <TextCropping width={width}
                         indexColumn={indexColumn}>{value ? showRawValue ? value.value : value.name : ''}</TextCropping>;
};

export default SelectValue;
