import React from 'react';
import { Form, Input } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';

const Text = ({
    value,
    name,
    onChange,
    onBlur,
    isDisabled,
    noLabel,
    className,
    text,
    error,
    type,
    datalist: valuesList = [],
    placeholder,
                  isRequired,
                  autoComplete,
}) => {
    const { t } = useTranslation();
    console.log('text');

    return (
        <Form.Field>
            {!noLabel ? (
                <label className={isDisabled ? 'label-disabled' : null}>{`${t(text || name)}${
                    isRequired ? ' *' : ''
                    }`}</label>
            ) : null}
            <Input
                placeholder={placeholder}
                list={valuesList && name}
                className={className}
                type={type}
                disabled={isDisabled}
                name={name}
                value={value}
                error={error}
                onChange={onChange}
                onBlur={onBlur}
                autoComplete={autoComplete}
            />
            {error && typeof error === 'string' ? (
                <span className="label-error">{error}</span>
            ) : null}
            {valuesList && valuesList.length ? (
                <datalist id={name}>
                    {valuesList.map(item => (
                        <option key={item.id} value={item.name} />
                    ))}
                </datalist>
            ) : null}
        </Form.Field>
    );
};
export default Text;
