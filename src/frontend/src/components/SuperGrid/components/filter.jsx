import React, { Component } from 'react';
import { Checkbox, Table, Input } from 'semantic-ui-react';
import { Resizable } from 'react-resizable';

import DateFacet from '../../FilterComponents/Date';
import TextFacet from '../../FilterComponents/Text';
import NumberFacet from '../../FilterComponents/Number';
import SelectFacet from '../../FilterComponents/Select';
import StateFacet from '../../FilterComponents/State';
import Bool from '../../FilterComponents/Bool';
import TimeFaset from '../../FilterComponents/Time';
import {
    DATE_TIME_TYPE,
    NUMBER_TYPE,
    SELECT_TYPE,
    STATE_TYPE,
    TEXT_TYPE,
    BOOLEAN_TYPE,
    DATE_TYPE,
    ENUM_TYPE,
    TIME_TYPE,
    LINK_TYPE,
} from '../../../constants/columnTypes';

const getTypeFacet = {
    [TEXT_TYPE]: <TextFacet />,
    [NUMBER_TYPE]: <NumberFacet />,
    [SELECT_TYPE]: <SelectFacet />,
    [DATE_TIME_TYPE]: <DateFacet />,
    [DATE_TYPE]: <DateFacet />,
    [TIME_TYPE]: <TimeFaset />,
    [STATE_TYPE]: <StateFacet />,
    [BOOLEAN_TYPE]: <Bool />,
    [ENUM_TYPE]: <SelectFacet />,
    [LINK_TYPE]: <TextFacet />,
};

const Control = props => {
    let params = {
        ...props.column,
        name: props.column.name,
        value: props.filters[props.column.name],
        setSort: props.setSort,
        onChange: props.setFilter,
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
    constructor(props) {
        super(props);

        /*const widths = [];

        props.columns.forEach(column => {
            column.name === 'status' ? widths.push(200) : widths.push(100);
        });*/

        this.state = {
            widths: [],
        };
    }

    /*componentDidUpdate (prevProps) {
        if(prevProps.columns !== this.props.columns) {
            const widths = [];

            this.props.columns.forEach(column => {
                column.name === 'status' ? widths.push(200) : widths.push(100);
            });

            this.state = {
                widths,
            };
        }
    }*/

    handleResize = (e, { size, index }) => {
        this.setState(({ widths }) => {
            const nextColumns = [...widths];
            nextColumns[index] = size.width;
            return { widths: nextColumns };
        });
    };

    render() {
        const {
            isShowActions,
            indeterminate,
            all,
            checkAllDisabled,
            setSelectedAll,
            columns,
        } = this.props;

        const { widths } = this.state;

        /*const columnStyle = column => ({
            maxWidth: column.width || 100 + 'px',
            minWidth: column.width || 100 + 'px',
        });*/

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
                        <Resizable
                            width={widths[i] || 100}
                            height={0}
                            onResize={(e, { size }) => this.handleResize(e, { size, index: i })}
                        >
                            <Table.HeaderCell
                                key={'th' + x.name + i}
                                style={{ maxWidth: `${widths[i]}px`, minWidth: `${widths[i]}px` }}
                                className={`column-facet column-${x.name
                                    .toLowerCase()
                                    .replace(' ', '-')}-facet`}
                            >
                                <Control key={'facet' + x.name} column={x} {...this.props} />
                            </Table.HeaderCell>
                        </Resizable>
                    ))}
                {isShowActions ? <Table.HeaderCell className="actions-column" /> : null}
            </Table.Row>
        );
    }
}

export default Filter;
