import React from 'react';
import { Form, TextArea } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';

const BigText = ({ value, name, onChange, isDisabled, noLabel, className, rows, text }) => {
    const { t } = useTranslation();

    return (
        <Form.Field>
            {!noLabel ? (
                <label className={isDisabled ? 'label-disabled' : null}>{t(text || name)}</label>
            ) : null}
            <TextArea
                className={className}
                autoHeight
                disabled={isDisabled || false}
                name={name}
                rows={rows}
                value={value || ''}
                onChange={onChange}
            />
        </Form.Field>
    );
};
export default BigText;
