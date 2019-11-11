import React, {useRef, useEffect, useState} from 'react';

const BodyCellComponent = ({column, children}) => {
    const cellRef = useRef(null);
    let [position, setPosition] = useState(null);

    useEffect(() => {
        console.log('cell', cellRef.current.offsetLeft);
        setPosition(cellRef.current.offsetLeft)
    }, [cellRef.current]);

    return (
        <td
            className={column.isFixedPosition ? "no-scroll no-scroll-value" : ""}
            ref={cellRef}
            style={column.isFixedPosition ? {left: position} : null}
        >
            {children}
        </td>
    );
};

export default BodyCellComponent;
