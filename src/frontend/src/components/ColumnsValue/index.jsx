import React from 'react';
import { useTranslation } from 'react-i18next';

import {
    ACTIVE_TYPE,
    BOOLEAN_TYPE,
    DATE_TIME_TYPE,
    ENUM_TYPE,
    NUMBER_TYPE,
    STATE_TYPE,
} from '../../constants/columnTypes';
import {formatDate} from '../../utils/dateTimeFormater';
import {numbersFormat} from '../../utils/numbersFormat';
import {Checkbox, Icon, Label} from 'semantic-ui-react';

const CellValue = ({ type, value = '', stateColors = [], id, toggleIsActive, isTranslate }) => {
    const { t } = useTranslation();

    if (type === ENUM_TYPE) {
        return (
            <>
                {!value
                    ? t('All')
                    : value.map((n, i) => (
                          <Label key={i} className="label-margin">
                              {n}
                          </Label>
                      ))}
            </>
        );
    }

    if (type === ACTIVE_TYPE) {
        return <Checkbox toggle itemID={id} checked={value} onChange={toggleIsActive} />;
    }

    if (type === BOOLEAN_TYPE) {
        return value ? 'Да' : 'Нет';
    }

    if (value === undefined || value === null) return '';

    if (type === NUMBER_TYPE) {
        return numbersFormat(parseFloat(value));
    }

   /* if (type === DATE_TIME_TYPE) {
        const dateString = formatDate(Date.parse(value), 'dd.MM.YYYY HH:mm').toString();
        return (
            <div
                key={`value_${id}`}
                dangerouslySetInnerHTML={{ __html: dateString.replaceAll(' ', '&nbsp;') }}
            />
        );
    }*/

    if (type === STATE_TYPE) {
        const state = stateColors.find(x => x.name === value);
        const color = state ? state.color : 'grey';

        return (
            <div>
                <Icon color={color.toLowerCase()} name="circle" />
                {t(value)}
            </div>
        );
    }

    return isTranslate ? t(value) : value
        .toString()
        .split(';')
        .map(z => (
            <div
                key={`value_${id}`}
                dangerouslySetInnerHTML={{ __html: z.replaceAll(' ', '&nbsp;') }}
            />
        ));
};

export default CellValue;
