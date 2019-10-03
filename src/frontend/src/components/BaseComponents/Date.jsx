import React from 'react';
import { Button, Grid, Label, Dropdown, Input, Popup, Form } from 'semantic-ui-react';
import DatePicker from 'react-datepicker';
import {formatDate, parseDate} from "../../utils/dateTimeFormater";
import { useTranslation } from 'react-i18next';

const Date = ({ value, name, onChange, isDisabled, noLabel, required, placeholder, className }) => {
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
                selected={parseDate((value || '').substring(0, 10))}
                locale={localStorage.getItem('i18nextLng')}
                dateFormat="dd.MM.yyyy"
                onChange={(date, e) => {
                    onChange(e, { name: name, value: date ? formatDate(date) : null });
                }}
            />
        </Form.Field>
    );
};
export default Date;
