import React from 'react';

import {
    ACTIVE_TYPE,
    BOOLEAN_TYPE,
    ENUM_TYPE,
    LABELS_TYPE,
    LINK_TYPE, LOCAL_DATE_TIME,
    NUMBER_TYPE,
    SELECT_TYPE,
    STATE_TYPE,
} from '../../constants/columnTypes';
import { numbersFormat } from '../../utils/numbersFormat';
import { Checkbox, Label } from 'semantic-ui-react';
import StateValue from './StateValue';
import SelectValue from './SelectValue';
import TextCropping from './TextCropping';
import {dateToUTC} from "../../utils/dateTimeFormater";

const CellValue = (
    {
        type,
        value = '',
        valueText,
        id,
        toggleIsActive,
        source,
        indexRow,
        indexColumn,
        modalCard,
        showRawValue,
        width,
        t,
        isDisabled
    }
) => {

    if (type === SELECT_TYPE) {
        return (
            <SelectValue
                width={width}
                value={value}
                valueText={valueText}
                source={source}
                indexRow={indexRow}
                indexColumn={indexColumn}
                showRawValue={showRawValue}
            />
        );
    }

    if (type === STATE_TYPE) {
        return (
            <StateValue
                width={width}
                value={value}
                source={source}
                indexRow={indexRow}
                indexColumn={indexColumn}
            />
        );
    }

    if (type === LABELS_TYPE) {
        return (
            <>
                {!value
                    ? t('All')
                    : value.map((n, i) => (
                        <Label key={i} className="label-margin">
                            {t(n.name)}
                        </Label>
                    ))}
            </>
        );
    }

    if (type === ENUM_TYPE) {
        return (
            <TextCropping width={width} indexColumn={indexColumn}>
                {value ? t(value.name) : ''}
            </TextCropping>
        );
    }

    if (type === ACTIVE_TYPE) {
        return <Checkbox toggle itemID={id} checked={value} disabled={isDisabled} onChange={toggleIsActive}/>;
    }

    if (type === BOOLEAN_TYPE) {
        return <>{value ? t('Yes') : t('No')}</>;
    }

    if (value === undefined || value === null) return '';

    if (type === NUMBER_TYPE) {
        return <>{numbersFormat(parseFloat(value))}</>;
    }

    if (type === LINK_TYPE) {
        return modalCard ? React.cloneElement(
            modalCard(),
            null,
            <div className="link-cell">
                <TextCropping width={width} indexColumn={indexColumn}>
                    {value}
                </TextCropping>
            </div>,
        ) : value;
    }

    if (type === LOCAL_DATE_TIME) {
        return (
            <TextCropping width={width} indexColumn={indexColumn}>
                {dateToUTC(value, 'DD.MM.YYYY HH:mm')}
            </TextCropping>
        );
    }

    return (
        <TextCropping width={width} indexColumn={indexColumn}>
            {value}
        </TextCropping>
    );
};

export default React.memo(CellValue);
