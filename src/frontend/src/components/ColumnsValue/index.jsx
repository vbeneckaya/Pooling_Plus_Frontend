import React from 'react';
import { useTranslation } from 'react-i18next';

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

const ModalComponent = ({ element, props, children }) => {
    if (!element) {
        return <>{children}</>;
    }
    return React.cloneElement(element, props, children);
};

const CellValue = React.forwardRef(
    (
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
            name,
            modalCard,
            showRawValue,
        },
        ref,
    ) => {
        const { t } = useTranslation();

        if (type === SELECT_TYPE) {
            return (
                <div ref={ref}>
                    <SelectValue
                        value={value}
                        source={source}
                        indexRow={indexRow}
                        showRawValue={showRawValue}
                    />
                </div>
            );
        }

        if (type === STATE_TYPE) {
            return (
                <div ref={ref}>
                    <StateValue value={value} source={source} indexRow={indexRow} />
                </div>
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
                <div ref={ref}>
                    <TextCropping>{t(value)}</TextCropping>
                </div>
            );
        }

        if (type === ACTIVE_TYPE) {
            return <Checkbox toggle itemID={id} checked={value} onChange={toggleIsActive} />;
        }

        if (type === BOOLEAN_TYPE) {
            return <div ref={ref}>{value ? t('Yes') : t('No')}</div>;
        }

        if (value === undefined || value === null) return '';

        if (type === NUMBER_TYPE) {
            return <div ref={ref}>{numbersFormat(parseFloat(value))}</div>;
        }

        if (type === LINK_TYPE) {
            return React.cloneElement(
                modalCard,
                null,
                <div className="link-cell" ref={ref}>
                    <TextCropping>{value}</TextCropping>
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
            <div ref={ref}>
                <TextCropping>{value}</TextCropping>
            </div>
        );
    },
);

export default CellValue;
