import React from 'react';
import { useTranslation } from 'react-i18next';

import {
    ACTIVE_TYPE,
    BOOLEAN_TYPE,
    DATE_TIME_TYPE,
    ENUM_TYPE,
    NUMBER_TYPE, SELECT_TYPE,
    STATE_TYPE,
} from '../../constants/columnTypes';
import {formatDate} from '../../utils/dateTimeFormater';
import {numbersFormat} from '../../utils/numbersFormat';
import {Checkbox, Icon, Label} from 'semantic-ui-react';
import {postman} from "../../utils/postman";
import StateValue from "./StateValue";

const CellValue = ({ type, value = '', stateColors = [], id, toggleIsActive, isTranslate, source }) => {
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

   /*if (type === SELECT_TYPE) {
       if (source && value) {
           postman.get(`/${source}/getById/${value}`).then(result => {
               console.log('result', result);
               return result.name;
           });
       }
   }*/

    if (type === STATE_TYPE) {
        return (
            <StateValue value={value} source={source} />
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
