import React from 'react';

import {
    ACTIVE_TYPE,
    DATE_TIME_TYPE,
    ENUM_TYPE,
    NUMBER_TYPE,
    STATE_TYPE,
    TEXT_TYPE,
} from '../../../constants/columnsType';
import { formatDate } from '../../../utils/dateTimeFormater';
import { numbersFormat } from '../../../utils/numbersFormat';
import { Icon, Label, Checkbox } from 'semantic-ui-react';

const CellValue = ({ type, value = '', stateColors = [], id, toggleIsActive }) => {
    if (type === ENUM_TYPE) {
        return (
            <>
                {!value
                    ? 'Все'
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

    if (typeof value === 'boolean') {
        return value ? 'Да' : 'Нет';
    }

    if (value === undefined || value === null) return '';

    if (type === NUMBER_TYPE) {
        return numbersFormat(value);
    }

    if (type === DATE_TIME_TYPE) {
        const dateString = formatDate(new Date(value), 'dd.MM.YYYY HH:mm').toString();
        return (
            <div
                key={`value_${id}`}
                dangerouslySetInnerHTML={{ __html: dateString.replaceAll(' ', '&nbsp;') }}
            />
        );
    }

    if (type === STATE_TYPE) {
        const state = stateColors.find(x => x.text === value.name);
        const color = state ? state.color : 'grey';

        return (
            <div>
                <Icon color={color} name="circle" />
                {value.name}
            </div>
        );
    }

    return value
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
