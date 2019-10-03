import React, { Component } from 'react';
import { Checkbox, Table, Input } from 'semantic-ui-react';

import DateFacet from '../../FilterComponents/Date';
import TextFacet from '../../FilterComponents/Text';
import NumberFacet from '../../FilterComponents/Number';
import SelectFacet from '../../FilterComponents/Select';
import StateFacet from '../../FilterComponents/State';

import {
    DATE_TIME_TYPE,
    NUMBER_TYPE,
    SELECT_TYPE,
    STATE_TYPE,
    TEXT_TYPE,
    BOOLEAN_TYPE, DATE_TYPE
} from '../../../constants/columnTypes';
import Bool from "../../BaseComponents/Bool";

const getTypeFacet = {
    [TEXT_TYPE]: <TextFacet />,
    [NUMBER_TYPE]: <NumberFacet />,
    [SELECT_TYPE]: <SelectFacet />,
    [DATE_TIME_TYPE]: <DateFacet />,
    [DATE_TYPE]: <DateFacet />,
    [STATE_TYPE]: <StateFacet />,
    [BOOLEAN_TYPE]: <Bool/>
};

const Control = props => {
    let params = {
        ...props.column,
        name: props.column.name,
        value: props.filters[props.column.name],
        setSort: props.setSort,
        onChange: props.setFilter,
        stateColors: props.stateColors,
    };

    if (props.sort && props.sort.name === props.column.name)
        params = {
            ...params,
            sort: props.sort.desc ? 'desc' : 'asc',
        };

    if (props.column.type === SELECT_TYPE) {
        params = {
            ...params,
            getLookup: props.getLookup,
        };
    }

    return React.cloneElement(getTypeFacet[props.column.type], params);
};

class Filter extends Component {
    render() {
        const {
            columns,
            isShowActions,
            indeterminate,
            all,
            checkAllDisabled,
            setSelectedAll,
        } = this.props;

        const columnStyle = column => ({
            maxWidth: column.width || 100 + 'px',
            minWidth: column.width || 100 + 'px',
        });

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
                {columns &&
                    columns.map((x, i) => (
                        <Table.HeaderCell
                            key={'th' + x.name + i}
                            style={columnStyle(x)}
                            className={`column-facet column-${x.name
                                .toLowerCase()
                                .replace(' ', '-')}-facet`}
                        >
                            <Control key={'facet' + x.name} column={x} {...this.props} />
                        </Table.HeaderCell>
                    ))}
                {isShowActions ? <Table.HeaderCell className="actions-column" /> : null}
            </Table.Row>
        );
    }
}

export default Filter;
