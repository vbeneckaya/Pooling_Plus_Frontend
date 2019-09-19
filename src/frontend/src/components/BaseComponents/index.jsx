import React from 'react';
import {TEXT_TYPE} from "../../constants/columnsType";
import Text from "./Text";

const getTypeFacet = {
    [TEXT_TYPE]: <Text/>
};

const FormField = props => {
    let params = {
        ...props.column,
        name: props.column.name,
        value: props.value,
        onChange: props.onChange
    };

    return React.cloneElement(getTypeFacet[props.column.type], params);
};

export default FormField;
