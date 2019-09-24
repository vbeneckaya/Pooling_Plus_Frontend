import React from 'react';
import { Form, Input } from 'semantic-ui-react';
import {useTranslation} from "react-i18next";

const Number = ({ value, name, onChange, isDisabled, noLabel, className }) => {
    const {t }=useTranslation();

    return (
        <Form.Field>
            {!noLabel ? (
                <label className={isDisabled ? 'label-disabled' : null}>{t(name)}</label>
            ) : null}
            <Input
                className={className}
                type="number"
                disabled={isDisabled || false}
                name={name}
                value={value || ''}
                onChange={onChange}
                autoComplete="off"
            />
        </Form.Field>
    );
};
export default Number;
