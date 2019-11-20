import React from 'react';
import {
    BIG_TEXT_TYPE,
    BOOLEAN_TYPE,
    CHECKBOX_TYPE,
    DATE_TIME_TYPE,
    DATE_TYPE,
    ENUM_TYPE,
    GROUP_TYPE,
    NUMBER_TYPE,
    SELECT_TYPE,
    STATE_TYPE,
    TEXT_TYPE,
    TIME_TYPE,
} from '../../constants/columnTypes';
import Text from './Text';
import TextArea from './TextArea';
import State from './State';
import Date from './Date';
import Select from './Select';
import Bool from './Bool';
import DateTime from './DateTime';
import { SETTINGS_TYPE_HIDE, SETTINGS_TYPE_SHOW } from '../../constants/formTypes';
import CheckBox from './Checkbox';

const getTypeFacet = {
    [TEXT_TYPE]: <Text />,
    [STATE_TYPE]: <State />,
    [DATE_TYPE]: <Date />,
    [DATE_TIME_TYPE]: <DateTime />,
    [TIME_TYPE]: <Text type="time" />,
    [SELECT_TYPE]: <Select />,
    [NUMBER_TYPE]: <Text />,
    [BOOLEAN_TYPE]: <Bool />,
    [ENUM_TYPE]: <Select isTranslate />,
    [BIG_TEXT_TYPE]: <TextArea />,
    [CHECKBOX_TYPE]: <CheckBox />,
};

const FormField = props => {
    let params = {
        ...props,
        ...props.column,
        type: props.typeValue,
    };

    if (props.type === SELECT_TYPE || (props.column && props.column.type === SELECT_TYPE)) {
        params = {
            ...params,
            source: props.source || (props.column && props.column.source),
        };
    }

    if (props.settings && props.settings === SETTINGS_TYPE_SHOW) {
        params = {
            ...params,
            isDisabled: true,
        };
    }

    if (props.settings && props.settings === SETTINGS_TYPE_HIDE) {
        params = {
            ...params,
            isDisabled: true,
            value: null,
        };
    }

    /* switch (props.type || (props.column && props.column.type)) {
         case TEXT_TYPE:
             return <Text {...params} />;
         case STATE_TYPE:
             return <State {...params} />;
         case DATE_TYPE:
             return <Date {...params} />;
         case DATE_TIME_TYPE:
             return <DateTime {...params} />;
         case TIME_TYPE:
             return <Text type="time" {...params} />;
         case SELECT_TYPE:
             return <Select {...params} />;
         case NUMBER_TYPE:
             return <Text {...params} />;
         case BOOLEAN_TYPE:
             return <Bool {...params} />;
         case ENUM_TYPE:
             return <Select isTranslate {...params} />;
         case BIG_TEXT_TYPE:
             return <TextArea {...params} />;
         case CHECKBOX_TYPE:
             return <CheckBox {...params} />;
         default:
             return <Text {...params} />
     }*/

    return React.cloneElement(
        getTypeFacet[props.type || (props.column && props.column.type)] || <TEXT_TYPE/>,
        params,
    );
};

export default React.memo(FormField, (prevProps, nextProps) => {
    return (
        prevProps.value === nextProps.value &&
        prevProps.type === nextProps.type &&
        prevProps.isDisabled === nextProps.isDisabled &&
        prevProps.error === nextProps.error
    );
});
