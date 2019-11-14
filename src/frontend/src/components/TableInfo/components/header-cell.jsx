import React, {useRef, useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

const HeaderCellComponent = ({row}) => {
    const {t} = useTranslation();
    const cellRef = useRef(null);
    let [position, setPosition] = useState(null);

    useEffect(() => {
        setPosition(cellRef.current.offsetLeft);
    }, []);

    console.log('headerCell');

    return (
        <th
            className={row.isFixedPosition ? 'no-scroll table-header-cell' : 'table-header-cell'}
            ref={cellRef}
            style={row.isFixedPosition ? {left: position} : null}
        >
            {t(row.name)}
        </th>
    );
};

export default React.memo(HeaderCellComponent, (prevProps, nextProps) => {
    return prevProps.row.name === nextProps.row.name
});
