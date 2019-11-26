import React, {useState} from 'react';
import {Icon, Input} from 'semantic-ui-react';

const PasswordField = ({value, onChange}) => {
    let [show, setShow] = useState(false);

    const toggle = () => {
        setShow(prevState => !prevState);
    };

    return (
        <Input
            value={value}
            fluid
            type={show ? 'text' : 'password'}
            icon={<Icon name={show ? 'eye slash' : 'eye'} link onClick={toggle}/>}
            autoComplete='new-password'
            onChange={onChange}
        />
    );
};

export default PasswordField;
