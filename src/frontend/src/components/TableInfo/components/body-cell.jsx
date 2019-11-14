import React, {useRef, useEffect, useState} from 'react';

const BodyCellComponent = ({column, children, value}) => {
    const cellRef = useRef(null);
    let [position, setPosition] = useState(null);
    let [width, setWidth] = useState(null);

    useEffect(() => {
        setPosition(cellRef.current.offsetLeft);
        setWidth(cellRef.current.offsetWidth);
    }, []);

    return (
        <td
            className={column.isFixedPosition ? "no-scroll no-scroll-value" : ""}
            ref={cellRef}
            style={column.isFixedPosition ?
                {
                    left: position,
                    maxWidth: width,
                    minWidth: width
                }
                : null}
        >
            {children}
        </td>
    );
};

export default BodyCellComponent;
