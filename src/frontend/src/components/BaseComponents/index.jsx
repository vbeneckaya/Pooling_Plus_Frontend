import React from 'react';
import {
    BOOLEAN_TYPE,
    DATE_TIME_TYPE,
    DATE_TYPE,
    ENUM_TYPE,
    GROUP_TYPE,
    NUMBER_TYPE,
    SELECT_TYPE,
    STATE_TYPE,
    TEXT_TYPE,
} from '../../constants/columnTypes';
import Text from './Text';
import State from './State';
import Date from './Date';
import Select from './Select';
import Bool from './Bool';
import DateTime from './DateTime';

const getTypeFacet = {
    [TEXT_TYPE]: <Text />,
    [STATE_TYPE]: <State />,
    [DATE_TYPE]: <Date />,
    [DATE_TIME_TYPE]: <DateTime />,
    [GROUP_TYPE]: <Text />,
    [SELECT_TYPE]: <Select />,
    [NUMBER_TYPE]: <Text />,
    [BOOLEAN_TYPE]: <Bool />,
    [ENUM_TYPE]: <Select isTranslate />,
};

const FormField = props => {
    let params = {
        ...props.column,
        name: props.column.name,
        value: props.value,
        onChange: props.onChange,
    };

    if (props.column.type === SELECT_TYPE) {
        params = {
            ...params,
            source: props.column.source,
        };
    }

    return React.cloneElement(getTypeFacet[props.column.type], params);
};

export default FormField;
