import React, {useEffect, useRef, useState, useCallback} from 'react';
import {Checkbox, Table} from 'semantic-ui-react';
import {Resizable} from 'react-resizable';
import FacetField from '../../FilterComponents';

const Filter = props => {
    const {
        isShowActions,
        indeterminate,
        all,
        checkAllDisabled,
        setSelectedAll,
        columns,
        resizeColumn,
        extWidth
    } = props;
    let [customColumns, setColumns] = useState(columns);
    let timer = useRef(null);

    useEffect(
        () => {
            setColumns(columns);

            return () => {
                timer.current = null;
            };
        },
        [columns],
    );

    const handleResize = useCallback((e, {size, index}) => {
        resizeColumn(size, index);
    }, []);

    console.log('extWidth', extWidth);

    return (
        <Table.Row className="sticky-header">
            <Table.HeaderCell className="small-column">
                <Checkbox
                    indeterminate={indeterminate}
                    checked={all}
                    disabled={checkAllDisabled}
                    onChange={setSelectedAll}
                />
            </Table.HeaderCell>
            {customColumns &&
            customColumns.map((x, i) => (
                <Resizable
                    key={`resizable_${x.name}`}
                    width={x.width}
                    height={0}
                    axis="x"
                    onResize={(e, {size}) => handleResize(e, {size, index: i})}
                >
                    <Table.HeaderCell
                        key={'th' + x.name}
                        style={{width: `${x.width}px`}}
                        className={`column-facet column-${x.name && x.name
                            .toLowerCase()
                            .replace(' ', '-')}-facet`}
                    >
                        <FacetField
                            key={'facet' + x.name}
                            index={i}
                            name={x.name}
                            sort={props.sort}
                            setSort={props.setSort}
                            type={x.type}
                            value={props.filters[x.name]}
                            setFilter={props.setFilter}
                            source={x.source}
                            width={x.width}
                            handleResize={handleResize}
                        />
                    </Table.HeaderCell>
                </Resizable>
            ))}
            <Table.HeaderCell style={{width: extWidth > 0 ? extWidth : 0}}/>
            {isShowActions ? <Table.HeaderCell className="actions-column"/> : null}
        </Table.Row>
    );
};

export default Filter;
