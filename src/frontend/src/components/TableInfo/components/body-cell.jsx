import React, {useRef, useEffect, useState} from 'react';

const BodyCellComponent = ({column, children}) => {
    const cellRef = useRef(null);
    let [position, setPosition] = useState(null);
    let [width, setWidth] = useState(null);

    useEffect(() => {
        setPosition(cellRef.current.offsetLeft);
        setWidth(cellRef.current.offsetWidth);
        return () => {
            console.log('clercell')
        }
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

export default React.memo(BodyCellComponent);
