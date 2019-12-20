import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form } from 'semantic-ui-react';
import DatePicker from 'react-datepicker';

import { formatDate, parseDateTime } from '../../utils/dateTimeFormater';

const DateTime = ({
    value,
    name,
    onChange,
    isDisabled,
    noLabel,
    popperPlacement = 'bottom-end',
    className,
    text,
    placeholder,
    isRequired,
    error,
}) => {
    const { t } = useTranslation();

    const getClassNames = () => {
        const classNames = [];

        if (error) {
            classNames.push('input-error');
        }

        if (className) {
            classNames.push(className);
        }

        return classNames.join(' ');
    };

    return (
        <Form.Field className={noLabel ? 'no-label-datepicker' : undefined}>
            {!noLabel ? (
                <label className={isDisabled ? 'label-disabled' : null}>{`${t(text || name)}${
                    isRequired ? ' *' : ''
                }`}</label>
            ) : null}
            <DatePicker
                placeholderText={placeholder}
                className={getClassNames()}
                locale={localStorage.getItem('i18nextLng')}
                disabled={isDisabled || false}
                isClearable={!(isDisabled || false)}
                selected={parseDateTime(value || '')}
                dateFormat="dd.MM.yyyy HH:mm"
                showTimeSelect
                timeFormat="HH:mm"
                timeIntervals={15}
                timeCaption={t('Time')}
                onChange={(date, e) => {
                    onChange(e, {
                        name: name,
                        value: date ? formatDate(date, 'dd.MM.yyyy HH:mm') : null,
                    });
                }}
                popperPlacement={popperPlacement}
                onChangeRaw={e => onChange(e, { name, value: e.target.value })}
            />
            {error && typeof error === 'string' ? (
                <span className="label-error">{error}</span>
            ) : null}
        </Form.Field>
    );
};
export default DateTime;
