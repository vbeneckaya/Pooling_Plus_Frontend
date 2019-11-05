import React from 'react';
import {
    BIG_TEXT_TYPE,
    BOOLEAN_TYPE,
    DATE_TIME_TYPE,
    DATE_TYPE,
    ENUM_TYPE,
    GROUP_TYPE,
    NUMBER_TYPE,
    SELECT_TYPE,
    STATE_TYPE,
    TEXT_TYPE,
    TIME_TYPE,
    CHECKBOX_TYPE
} from '../../constants/columnTypes';
import Text from './Text';
import TextArea from './TextArea';
import State from './State';
import Date from './Date';
import Select from './Select';
import Bool from './Bool';
import DateTime from './DateTime';
import {SETTINGS_TYPE_HIDE, SETTINGS_TYPE_SHOW} from '../../constants/formTypes';
import CheckBox from "./Checkbox";

const getTypeFacet = {
    [TEXT_TYPE]: <Text />,
    [STATE_TYPE]: <State />,
    [DATE_TYPE]: <Date />,
    [DATE_TIME_TYPE]: <DateTime />,
    [TIME_TYPE]: <Text type="time"/>,
    [GROUP_TYPE]: <Text />,
    [SELECT_TYPE]: <Select />,
    [NUMBER_TYPE]: <Text />,
    [BOOLEAN_TYPE]: <Bool />,
    [ENUM_TYPE]: <Select isTranslate />,
    [BIG_TEXT_TYPE]: <TextArea/>,
    [CHECKBOX_TYPE]: <CheckBox/>
};

const FormField = props => {
    if (props.settings === SETTINGS_TYPE_HIDE) {
        return null;
    }

    let params = {
        ...props,
        ...props.column,
    };

    if (props.type === SELECT_TYPE || (props.column && props.column.type === SELECT_TYPE)) {
        params = {
            ...params,
            source: props.source || (props.column && props.column.source),
        };
    }

    if (props.settings === SETTINGS_TYPE_SHOW) {
        params = {
            ...params,
            isDisabled: true,
        };
    }

    return React.cloneElement(
        getTypeFacet[props.type || (props.column && props.column.type)],
        params,
    );
};

export default FormField;
