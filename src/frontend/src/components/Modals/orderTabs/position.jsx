import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Button, Form, Grid, Icon, Table } from 'semantic-ui-react';
import TableInfo from '../../TableInfo';
import Text from '../../BaseComponents/Text';
import { useSelector, useDispatch } from 'react-redux';
import {cardSelector, editCardRequest} from '../../../ducks/gridCard';

const EditField = ({ value, name, onChange }) => {
    return <Text value={value} name={name} onChange={onChange} noLabel/>;
};

const columns = [
    {
        name: 'nart',
    },
    {
        name: 'description',
    },
    {
        name: 'country',
    },
    {
        name: 'spgr',
    },
    {
        name: 'ean',
    },
    {
        name: 'expirationDate',
    },
    {
        name: 'weight',
    },
    {
        name: 'netWeight',
    },
    {
        name: 'quantity',
    },
];

const Position = ({ form, onChange, gridName, load }) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    let [items, setItems] = useState([...form.items]);
    let [indexEdit, setIndexEdit] = useState(null);

    const card = useSelector(state => cardSelector(state));

    useEffect(() => {
        setItems(form.items);
    }, [form.items]);

    const editPositions = (positions) => {
        dispatch(editCardRequest({
            name: gridName,
            params: {
                ...card,
                items: positions
            },
            callbackSuccess: load
        }));
    };

    const handleDeleteItem = index => {
        const newItems = items.filter((item, i) => i !== index);

        editPositions(newItems);
    };

    const handleEditItem = index => {
        setIndexEdit(index);
    };

    const handleSaveItem = () => {
        editPositions(items);

        setIndexEdit(null);
    };

    const handleChangeField = (e, {name, value})=> {
        const newColumns = [...items];

        newColumns[indexEdit] = {
            ...items[indexEdit],
            [name]: value
        };

        setItems(newColumns)
    };

    const handleCancelItem = (index) => {
        setItems([...card.items]);
        setIndexEdit(null);
    };

    const handleAddItems = () => {
        setItems([...items, {}]);
        setIndexEdit(items.length);
    };

    return (
        <>
            <Grid>
                <Grid.Row columns="equal">
                    <Grid.Column width={4}>
                        <Form>
                            <Text
                                name="orderAmountExcludingVAT"
                                value={form['orderAmountExcludingVAT']}
                                onChange={onChange}
                            />
                        </Form>
                    </Grid.Column>
                    <Grid.Column className="add-right-elements">
                        <Button onClick={handleAddItems}>{t('AddButton')}</Button>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row>
                    <Table className="wider container-margin-top-bottom">
                        <Table.Header>
                            <Table.Row>
                                {columns.map(column => (
                                    <Table.HeaderCell key={column.name}>
                                        {t(column.name)}
                                    </Table.HeaderCell>
                                ))}
                                <Table.HeaderCell />
                            </Table.Row>
                        </Table.Header>
                        <Table.Body>
                            {items.map((row, index) => (
                                <Table.Row key={row.id}>
                                    {columns.map(column => (
                                        <>
                                            {index === indexEdit &&
                                            (column.name === 'nart' ||
                                                column.name === 'quantity') ? (
                                                <Table.Cell
                                                    key={`cell_${row.id}_${column.name}_${index}`}
                                                    className={`table-edit-field-${column.name}`}
                                                >
                                                    <EditField value={row[column.name]} name={column.name} onChange={handleChangeField} />
                                                </Table.Cell>
                                            ) : (
                                                <Table.Cell
                                                    key={`cell_${row.id}_${column.name}_${index}`}
                                                >
                                                    {row[column.name]}
                                                </Table.Cell>
                                            )}
                                        </>
                                    ))}
                                    <Table.Cell textAlign="right">
                                        {index === indexEdit ? (
                                            <>
                                                <Button icon onClick={() => handleCancelItem(index)}>
                                                    <Icon name="undo alternate" />
                                                </Button>
                                                <Button icon onClick={() => handleSaveItem(index)}>
                                                    <Icon name="check" />
                                                </Button>
                                            </>
                                        ) : (
                                            <>
                                                <Button icon onClick={() => handleEditItem(index)}>
                                                    <Icon name="pencil alternate" />
                                                </Button>
                                                <Button
                                                    icon
                                                    onClick={() => handleDeleteItem(index)}
                                                >
                                                    <Icon name="trash alternate" />
                                                </Button>
                                            </>
                                        )}
                                    </Table.Cell>
                                </Table.Row>
                            ))}
                        </Table.Body>
                    </Table>
                </Grid.Row>
            </Grid>
        </>
    );
};

export default Position;
