import React from 'react';
import {
    DATE_TYPE,
    GROUP_TYPE,
    NUMBER_TYPE,
    SELECT_TYPE,
    STATE_TYPE,
    TEXT_TYPE,
} from '../../constants/columnTypes';
import Text from './Text';
import State from './State';
import Date from './Date';

const getTypeFacet = {
    [TEXT_TYPE]: <Text />,
    [STATE_TYPE]: <State />,
    [DATE_TYPE]: <Date />,
    [GROUP_TYPE]: <Text />,
    [SELECT_TYPE]: <Text />,
    [NUMBER_TYPE]: <Text />,
};

const FormField = props => {
    let params = {
        ...props.column,
        name: props.column.name,
        value: props.value,
        onChange: props.onChange,
    };

    return React.cloneElement(getTypeFacet[props.column.type], params);
};

export default FormField;
