import React, {useRef, useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

const HeaderCellComponent = ({row}) => {
    const {t} = useTranslation();
    const cellRef = useRef(null);
    let [position, setPosition] = useState(null);

    useEffect(() => {
        console.log('cell', cellRef.current.offsetLeft);
        setPosition(cellRef.current.offsetLeft)
    }, [cellRef.current]);

    return (
        <th
            className={row.isFixedPosition ? 'no-scroll table-header-cell' : 'table-header-cell'}
            key={row.name}
            ref={cellRef}
            style={row.isFixedPosition ? {left: position} : null}
        >
            {t(row.name)}
        </th>
    );
};

export default HeaderCellComponent;
