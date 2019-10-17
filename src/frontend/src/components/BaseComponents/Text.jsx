import React from "react";
import {Form, Input} from "semantic-ui-react";
import { useTranslation } from 'react-i18next';

const Text = ({ value, name, onChange, isDisabled, noLabel, className, text, error, type, datalist: valuesList = [], errorText, placeholder }) => {
    const { t } = useTranslation();

    return (
        <Form.Field>
            {!noLabel ? <label className={isDisabled ? "label-disabled" : null}>{t(text || name)}</label> : null}
            <Input placeholder={placeholder} list={valuesList && name} className={className} type={type} disabled={isDisabled} name={name} value={value || ""} error={error} onChange={onChange} />
            {errorText && <span className="label-error">{errorText}</span>}
            {
               valuesList && valuesList.length ?
                <datalist id={name}>
                    {
                        valuesList.map(item => (
                            <option key={item.id} value={item.name}/>
                        ))
                    }
                </datalist> : null
            }
        </Form.Field>
    );
};
export default Text;
