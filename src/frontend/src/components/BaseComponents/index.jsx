import React, { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import {
    BIG_TEXT_TYPE,
    BOOLEAN_TYPE,
    CHECKBOX_TYPE,
    DATE_TIME_TYPE,
    DATE_TYPE,
    ENUM_TYPE,
    GROUP_TYPE,
    LOCAL_DATE_TIME,
    NUMBER_TYPE,
    SELECT_TYPE,
    SOLD_TO_TYPE,
    STATE_TYPE,
    TEXT_TYPE,
    TIME_TYPE,
    PASSWORD_TYPE,
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
import { clearError } from '../../ducks/gridCard';
import SoldToField from './SoldToField';
import PasswordField from "./Password";

const getTypeFacet = {
    [TEXT_TYPE]: <Text />,
    [STATE_TYPE]: <State />,
    [DATE_TYPE]: <Date />,
    [DATE_TIME_TYPE]: <DateTime />,
    [LOCAL_DATE_TIME]: <DateTime />,
    [TIME_TYPE]: <Text type="time" />,
    [SELECT_TYPE]: <Select />,
    [NUMBER_TYPE]: <Text />,
    [BOOLEAN_TYPE]: <Bool />,
    [ENUM_TYPE]: <Select isTranslate />,
    [BIG_TEXT_TYPE]: <TextArea />,
    [CHECKBOX_TYPE]: <CheckBox />,
    [SOLD_TO_TYPE]: <SoldToField />,
    [PASSWORD_TYPE]: <PasswordField/>
};

const FormField = props => {
    const dispatch = useDispatch();

    let params = {
        ...props,
        type: props.typeValue,
    };

    if ((props.settings && props.settings === SETTINGS_TYPE_SHOW) || props.isReadOnly) {
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

    useEffect(() => {
        if (props.error) {
            dispatch(clearError && clearError(props.name));
        }
    }, [props.value]);

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
        getTypeFacet[props.type || (props.column && props.column.type)] || <TEXT_TYPE />,
        params,
    );
};

export default React.memo(FormField, (prevProps, nextProps) => {
    return (
        prevProps.value === nextProps.value &&
        prevProps.type === nextProps.type &&
        prevProps.settings === nextProps.settings &&
        prevProps.isDisabled === nextProps.isDisabled &&
        prevProps.error === nextProps.error
    );
});
