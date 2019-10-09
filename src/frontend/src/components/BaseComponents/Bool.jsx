import React from 'react';
import { Form, Input, Radio } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';

const Bool = ({ value, name, onChange, isDisabled, noLabel, className, text, error }) => {
    const { t } = useTranslation();

    return (
        <Form.Field>
            {!noLabel ? (
                <label className={isDisabled ? 'label-disabled' : null}>{t(text || name)}</label>
            ) : null}
            {/*<Input className={className} disabled={isDisabled} name={name} value={value || ""} error={error} onChange={onChange} autoComplete="off" />*/}
            <div className="bool-radio-button">
                <Radio
                    label={t('Yes')}
                    name={name}
                    value={true}
                    checked={value === true}
                    className={className}
                    disabled={isDisabled}
                    onChange={onChange}
                />
                <Radio
                    label={t('No')}
                    name={name}
                    value={false}
                    checked={value === false}
                    className={className}
                    disabled={isDisabled}
                    onChange={onChange}
                />
            </div>
        </Form.Field>
    );
};
export default Bool;
