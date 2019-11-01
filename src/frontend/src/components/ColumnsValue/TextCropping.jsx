import React, {useRef, useEffect, useState} from 'react';
import {Icon, Popup} from 'semantic-ui-react';
import {checkForEditingRequest} from '../../ducks/gridColumnEdit';
import {toast} from 'react-toastify';

const TextCropping = ({children}) => {
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
