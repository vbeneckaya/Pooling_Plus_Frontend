import React from 'react';
import {useTranslation} from "react-i18next";
import {Form} from 'semantic-ui-react';
import DatePicker from 'react-datepicker';

import {formatDate, parseDateTime} from "../../utils/dateTimeFormater";

const DateTime = ({ value, name, onChange, isDisabled, noLabel, popperPlacement="bottom-end", className, text, placeholder }) => {
    const { t } = useTranslation();

    return (
        <Form.Field className={noLabel ? 'no-label-datepicker' : undefined}>
            {!noLabel ? (
                <label className={isDisabled ? 'label-disabled' : null}>{t(text || name)}</label>
            ) : null}
            <DatePicker
                placeholderText={placeholder}
                locale={localStorage.getItem('i18nextLng')}
                disabled={isDisabled || false}
                isClearable={!(isDisabled || false)}
                selected={parseDateTime(value || '')}
                dateFormat="dd.MM.yyyy HH:mm"
                className={className}
                showTimeSelect
                timeFormat="HH:mm"
                timeIntervals={15}
                timeCaption={t('Time')}
                onChange={(date, e) => {
                    onChange(e, { name: name, value: date ? formatDate(date, 'dd.MM.yyyy HH:mm') : null });
                }}
                popperPlacement={popperPlacement}
                onChangeRaw={e => onChange(e, { name, value: e.target.value })}
            />
        </Form.Field>
    );
};
export default DateTime;
