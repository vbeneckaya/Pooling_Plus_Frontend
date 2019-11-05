import React, { useEffect, useRef, useState } from 'react';
import { Popup } from 'semantic-ui-react';

const TextCropping = ({ children }) => {
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
        [valueRef, children],
    );

    return (
        <Popup
            content={children}
            disabled={width.scrollWidth <= width.offsetWidth}
            trigger={
                <div className="column-overflow" ref={valueRef}>
                    {children}
                </div>
            }
        />
    );
};

export default TextCropping;
