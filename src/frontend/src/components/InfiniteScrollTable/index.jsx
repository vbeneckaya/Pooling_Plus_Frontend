import React from 'react';

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
}) => {
    return (
        <div style={{ position: 'relative', ...style }}>
            <Table
                celled={celled === undefined ? true : celled}
                selectable={selectable === undefined ? true : celled}
                unstackable={unstackable || false}
                structured={structured}
                className={className || ''}
            >
                <Table.Header>{headerRow}</Table.Header>

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

export default InfiniteScrollTable;
