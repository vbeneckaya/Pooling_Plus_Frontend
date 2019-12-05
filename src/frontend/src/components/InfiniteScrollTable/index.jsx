import React, {useEffect, useRef, useState} from 'react';

import { Table, Visibility } from 'semantic-ui-react';

import './style.scss';

const InfiniteScrollTable = ({
    children,
    header,
    headerRow,
    onBottomVisible,
    unstackable,
    celled,
    selectable,
    className,
    context,
    style,
    structured,
                                 fixed,
                                 columns = [],
}) => {
    const tableRef = useRef(null);
    let [width, setWidth] = useState(0);
    let [extWidth, setExtWidth] = useState();

    useEffect(
        () => {
            let sum = 0;

            columns.forEach(item => {
                sum = sum + item.width;
            });
            setExtWidth(tableRef.current.scrollWidth - sum);
            setWidth(sum + 50);
        },
        [columns],
    );

    return (
        <div style={{position: 'relative', ...style}} ref={tableRef}>
            <Table
                celled={celled === undefined ? true : celled}
                selectable={selectable === undefined ? true : celled}
                unstackable={unstackable || false}
                structured={structured}
                className={className || ''}
                fixed={fixed}
                style={{width: '100%', minWidth: width}}
            >
                <Table.Header>{React.cloneElement(headerRow, {extWidth})}</Table.Header>

                {children}
            </Table>
            <Visibility
                continuous={false}
                once={false}
                context={context || undefined}
                onTopVisible={onBottomVisible}
                style={{
                    position: 'absolute',
                    bottom: 0,
                    left: 0,
                    right: 0,
                    zIndex: -1,
                }}
            />
        </div>
    );
};

export default React.memo(InfiniteScrollTable);
