import React from 'react';
import { Button, Grid, Label, Dropdown, Input, Popup, Form } from 'semantic-ui-react';
import DatePicker from 'react-datepicker';
import {formatDate, parseDate} from "../../utils/dateTimeFormater";
import { useTranslation } from 'react-i18next';

const DateF = ({ value, name, onChange, isDisabled, noLabel, required, placeholder, className }) => {
    const getClassNames = () => {
        const classNames = [];

        if (required && !value) {
            classNames.push('input-error');
        }

        if (className) {
            classNames.push(className);
        }

        return classNames.join(' ');
    };

    const { t } = useTranslation();

    return (
        <Form.Field>
            {!noLabel ? (
                <label>{t(name)}</label>
            ) : null}
            <DatePicker
                disabled={isDisabled || false}
                placeholderText={placeholder}
                className={getClassNames()}
                isClearable={!(isDisabled || false)}
                selected={value ? Date.parse(value) : null}
                locale={localStorage.getItem('i18nextLng')}
                dateFormat="dd.MM.yyyy"
                onChange={(date, e) => {
                    onChange(e, { name: name, value: date ? date.toISOString() : null });
                }}
            />
        </Form.Field>
    );
};
export default DateF;
