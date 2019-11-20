import React from 'react';
import { Form, Input } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';

const Number = ({
    value,
    name,
    onChange,
    isDisabled,
    noLabel,
    className,
    text,
    error,
    placeholder,
                    isRequired,
}) => {
    const { t } = useTranslation();

    return (
        <Form.Field>
            {!noLabel ? (
                <label className={isDisabled ? 'label-disabled' : null}>{`${t(text || name)}${
                    isRequired ? ' *' : ''
                    }`}</label>
            ) : null}
            <Input
                placeholder={placeholder}
                className={className}
                type="number"
                error={error}
                disabled={isDisabled || false}
                name={name}
                value={value}
                onChange={onChange}
                autoComplete="off"
            />
            {error && typeof error === 'string' ? <span className="label-error">{error}</span> : null}
        </Form.Field>
    );
};
export default Number;
