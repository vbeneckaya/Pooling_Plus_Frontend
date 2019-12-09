import React, {useRef, useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

const HeaderCellComponent = ({row}) => {
    const {t} = useTranslation();
    const cellRef = useRef(null);
    let [position, setPosition] = useState(null);
    let [width, setWidth] = useState(null);

    useEffect(() => {
        setPosition(cellRef.current.offsetLeft);
        setWidth(cellRef.current.offsetWidth);
    }, []);

    return (
        <th
            className={row.isFixedPosition ? 'no-scroll table-header-cell' : 'table-header-cell'}
            ref={cellRef}
            style={
                row.isFixedPosition
                    ? {
                        left: position,
                        maxWidth: '150px',
                        minWidth: '150px',
                    }
                    : null
            }
        >
            {t(row.name)}
        </th>
    );
};

export default React.memo(HeaderCellComponent);
