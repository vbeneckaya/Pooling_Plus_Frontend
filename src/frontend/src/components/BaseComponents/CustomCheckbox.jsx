import React from 'react';
import Icon from "../CustomIcon";

const CustomCheckbox = ({checked, onChange, disabled, all, indeterminate}) => {
    const handleOnChange = (e) => {
        console.log('7777')
        onChange()
    };

    let icon = '';

    if (checked) {
        icon = "checkbox"
    } else {
        icon = "checkbox-empty"
    }

    return (
        <div className="custom-checkbox">
            <Icon name={checked ? "checkbox" : "checkbox-empty"} size="2"
                  color={checked ? "#18a8cc" : "rgba(0, 0, 0, 0.4)"}/>
            <input type="checkbox" checked={checked} disabled={disabled} onClick={handleOnChange}/>
        </div>
    )
};

export default CustomCheckbox;
