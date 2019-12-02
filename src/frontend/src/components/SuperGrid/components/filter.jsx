import React, {useState, useEffect, useRef} from 'react';
import { Checkbox, Table } from 'semantic-ui-react';
import { Resizable } from 'react-resizable';
import FacetField from '../../FilterComponents';
import {debounce} from 'throttle-debounce';
import {
    editRepresentationRequest,
    getRepresentationsRequest,
    representationNameSelector,
} from '../../../ducks/representations';
import {useDispatch, useSelector} from 'react-redux';

const Filter = props => {
    const {
        isShowActions,
        indeterminate,
        all,
        checkAllDisabled,
        setSelectedAll,
        columns,
        gridName,
    } = props;
    const dispatch = useDispatch();
    let [customColumns, setColumns] = useState(columns);
    const representationName = useSelector(state => representationNameSelector(state, gridName));
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

    const handleResize = (e, {size, index}) => {
        clearTimeout(timer.current);
        const nextColumns = [...customColumns];
        nextColumns[index] = {
            ...nextColumns[index],
            width: size.width,
        };
        setColumns(nextColumns);

        /* debounce(300, dispatch(editRepresentationRequest({
             key: gridName,
             name: representationName,
             oldName: representationName,
             value: nextColumns,
         })))*/
        timer.current = setTimeout(() => {
            dispatch(
                editRepresentationRequest({
                    key: gridName,
                    name: representationName,
                    oldName: representationName,
                    value: nextColumns,
                    callbackSuccess: () => {
                        dispatch(getRepresentationsRequest({key: gridName}));
                    },
                }),
            );
        }, 2000);
    };

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
                    onResize={(e, {size}) => handleResize(e, {size, index: i})}
                >
                    <Table.HeaderCell
                        key={'th' + x.name + i}
                        style={{maxWidth: `${x.width}px`, minWidth: `${x.width}px`}}
                        className={`column-facet column-${x.name
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
                        />
                    </Table.HeaderCell>
                </Resizable>
            ))}
            {isShowActions ? <Table.HeaderCell className="actions-column"/> : null}
        </Table.Row>
    );
};

export default Filter;
