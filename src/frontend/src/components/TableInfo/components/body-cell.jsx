import React, {useRef, useEffect, useState} from 'react';
import CellValue from "../../ColumnsValue";
import TextCropping from "../../ColumnsValue/TextCropping";

const BodyCellComponent = ({column, children, value, indexColumn, indexRow, toggleIsActive}) => {
    const cellRef = useRef(null);
    let [position, setPosition] = useState(null);
    let [width, setWidth] = useState(null);

    useEffect(() => {
        setPosition(cellRef.current.offsetLeft);
        setWidth(cellRef.current.offsetWidth);
    }, []);

    /*console.log('cell');*/

    return (
        <td
            className={column.isFixedPosition ? 'no-scroll no-scroll-value' : ''}
            ref={cellRef}
            style={
                column.isFixedPosition
                    ? {
                        left: position,
                        maxWidth: width,
                        minWidth: width,
                    }
                    : null
            }
        >
            <CellValue
                {...column}
                toggleIsActive={toggleIsActive}
                indexRow={indexRow}
                indexColumn={indexColumn}
                value={value}
            />
            {/*<TextCropping width={width} indexColumn={indexColumn}>
                {value}
            </TextCropping>*/}
        </td>
    );
};

export default React.memo(BodyCellComponent, (prevProps, nextProps) => {
    return prevProps.value === nextProps.value;

});
