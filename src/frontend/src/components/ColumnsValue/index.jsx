import React, {useRef} from 'react';
import { useTranslation } from 'react-i18next';

import {
    ACTIVE_TYPE,
    BOOLEAN_TYPE,
    DATE_TIME_TYPE,
    ENUM_TYPE, LABELS_TYPE, LINK_TYPE,
    NUMBER_TYPE, SELECT_TYPE,
    STATE_TYPE,
} from '../../constants/columnTypes';
import {formatDate} from '../../utils/dateTimeFormater';
import {numbersFormat} from '../../utils/numbersFormat';
import {Checkbox, Icon, Label, Popup} from 'semantic-ui-react';
import {postman} from "../../utils/postman";
import StateValue from "./StateValue";
import SelectValue from "./SelectValue";

const ModalComponent = ({ element, props, children }) => {
    if (!element) {
        return <>{children}</>;
    }
    return React.cloneElement(element, props, children);
};

const CellValue = ({ type, value = '', stateColors = [], id, key_id, toggleIsActive, isTranslate, source, indexRow, name, modalCard, showRawValue }) => {
    const { t } = useTranslation();
    const addressRef = useRef(null);

    if (type === SELECT_TYPE) {
        return <SelectValue value={value} source={source} indexRow={indexRow} showRawValue={showRawValue} />
    }

    if (type === STATE_TYPE) {
        return (
            <StateValue value={value} source={source} indexRow={indexRow} />
        );
    }

    if (type === LABELS_TYPE) {
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

    if (type === ENUM_TYPE) {
        return <>{t(value)}</>
    }

    if (type === ACTIVE_TYPE) {
        console.log('id');
        return <Checkbox toggle itemID={id} checked={value} onChange={toggleIsActive} />;
    }

    if (type === BOOLEAN_TYPE) {
        return value ? t('Yes') : t('No');
    }

    if (value === undefined || value === null) return '';

    if (type === NUMBER_TYPE) {
        return numbersFormat(parseFloat(value));
    }

    if (type === LINK_TYPE) {
        return React.cloneElement(modalCard, null, <div className="link-cell">{value}</div>)
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

   if (name.toLowerCase().includes('address')) {
       const addressBlock = addressRef.current;
       const scrollWidth = addressBlock && addressRef.current.scrollWidth;
       const offsetWidth = addressBlock && addressRef.current.offsetWidth;
       return (
           <Popup
               content={value}
               disabled={scrollWidth <= offsetWidth}
               trigger={<div className="column-address" ref={addressRef}>
                   {value}
               </div>}
           />
       );
   }

    return isTranslate ? t(value) : value
        .toString()
        .split(';')
        .map(z => (
            <div
                key={`value_${key_id}`}
                dangerouslySetInnerHTML={{ __html: z.replaceAll(' ', '&nbsp;') }}
            />
        ));
};

export default CellValue;
