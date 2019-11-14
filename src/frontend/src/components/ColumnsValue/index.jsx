import React from 'react';

import {
    ACTIVE_TYPE,
    BOOLEAN_TYPE,
    ENUM_TYPE,
    LABELS_TYPE,
    LINK_TYPE,
    NUMBER_TYPE,
    SELECT_TYPE,
    STATE_TYPE,
} from '../../constants/columnTypes';
import { numbersFormat } from '../../utils/numbersFormat';
import { Checkbox, Label } from 'semantic-ui-react';
import StateValue from './StateValue';
import SelectValue from './SelectValue';
import TextCropping from './TextCropping';

const CellValue = (
    {
        type,
        value = '',
        stateColors = [],
        id,
        key_id,
        toggleIsActive,
        isTranslate,
        source,
        indexRow,
        indexColumn,
        name,
        modalCard,
        showRawValue,
        width,
        t
    }
) => {

    if (type === SELECT_TYPE) {
        return (
            <SelectValue
                width={width}
                value={value}
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
                {t(value)}
            </TextCropping>
        );
    }

    if (type === ACTIVE_TYPE) {
        return <Checkbox toggle itemID={id} checked={value} onChange={toggleIsActive}/>;
    }

    if (type === BOOLEAN_TYPE) {
        return <>{value ? t('Yes') : t('No')}</>;
    }

    if (value === undefined || value === null) return '';

    if (type === NUMBER_TYPE) {
        return <>{numbersFormat(parseFloat(value))}</>;
    }

    if (type === LINK_TYPE) {
        return React.cloneElement(
            modalCard,
            null,
            <div className="link-cell">
                <TextCropping width={width} indexColumn={indexColumn}>
                    {value}
                </TextCropping>
            </div>,
        );
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

    return (
        <TextCropping width={width} indexColumn={indexColumn}>
            {value}
        </TextCropping>
    );
};

export default CellValue;
