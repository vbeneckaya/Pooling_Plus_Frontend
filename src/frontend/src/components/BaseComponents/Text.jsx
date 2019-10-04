import React from "react";
import {Form, Input} from "semantic-ui-react";
import { useTranslation } from 'react-i18next';

const Text = ({ value, name, onChange, isDisabled, noLabel, className, text, error }) => {
    const { t } = useTranslation();

    return (
        <Form.Field>
            {!noLabel ? <label className={isDisabled ? "label-disabled" : null}>{t(text || name)}</label> : null}
            <Input className={className} disabled={isDisabled} name={name} value={value || ""} error={error} onChange={onChange} autoComplete="off" />
        </Form.Field>
    );
};
export default Text;
