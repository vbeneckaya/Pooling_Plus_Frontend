import React, {useEffect, useRef, useState, useCallback} from 'react';
import {Checkbox, Table} from 'semantic-ui-react';
import {Resizable} from 'react-resizable';
import FacetField from '../../FilterComponents';
import CustomCheckbox from "../../BaseComponents/CustomCheckbox";

const Filter = props => {
    const {
        isShowActions,
        indeterminate,
        all,
        checkAllDisabled,
        setSelectedAll,
        columns,
        resizeColumn,
        extWidth,
    } = props;

    const handleResize = useCallback((e, {size, index}) => {
        resizeColumn(size, index);
    }, []);

    console.log('columns', columns);

    return (
        <Table.Row className="sticky-header">
            <Table.HeaderCell className="small-column">
                <CustomCheckbox
                    indeterminate={indeterminate}
                    checked={all}
                    multi
                    disabled={checkAllDisabled}
                    onChange={setSelectedAll}
                />
            </Table.HeaderCell>
            {columns &&
            columns.map((x, i) => (
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
                        className={`column-facet column-${x.name &&
                        x.name.toLowerCase().replace(' ', '-')}-facet`}
                    >
                        <FacetField
                            key={'facet' + x.name}
                            index={i}
                            name={x.name}
                            displayNameKey={x.displayNameKey}
                            sort={x.sort}
                            setSort={props.setSort}
                            type={x.type}
                            value={x.filter}
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
