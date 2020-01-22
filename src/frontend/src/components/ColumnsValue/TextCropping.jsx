import React, { useEffect, useRef, useState } from 'react';
import { Popup } from 'semantic-ui-react';
import {useSelector} from 'react-redux';
import {representationSelector} from '../../ducks/representations';

const TextCropping = ({children, width: columnWidth, indexColumn}) => {
    const valueRef = useRef(null);
    let [width, setWidth] = useState({
        scrollWidth: 0,
        offsetWidth: 0,
    });

    useEffect(
        () => {
            setWidth({
                scrollWidth: valueRef.current && valueRef.current.scrollWidth,
                offsetWidth: valueRef.current && valueRef.current.offsetWidth,
            });
        },
        [valueRef.current, columnWidth, children],
    );

    // console.log('columnWidth', columnWidth, width)

    return (
        <Popup
            content={children}
            context={valueRef}
            disabled={width.scrollWidth <= width.offsetWidth}
            position={indexColumn === 0 ? 'top left' : 'top right'}
            trigger={
                <div className="column-overflow" ref={valueRef}>
                    {children}
                </div>
            }
        />
    );
};

export default TextCropping;
