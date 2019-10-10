import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Button, Form, Grid, Icon, Table } from 'semantic-ui-react';
import TableInfo from '../../TableInfo';
import Text from '../../BaseComponents/Text';

const EditField = ({ value }) => {
    return <Text value={value} />;
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

const Position = ({ form, onChange }) => {
    const { t } = useTranslation();
    let [items, setItems] = useState([...form.items]);
    let [editItem, setEditItem] = useState({});
    let [indexEdit, setIndexEdit] = useState(null);

    useEffect(
        () => {
            setItems(form.items);
        },
        [form.items],
    );

    const handleDeleteItem = index => {};

    const handleEditItem = index => {
        setIndexEdit(index);
    };

    const handleSaveItem = () => {

    }

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
                        <Button>{t('AddButton')}</Button>
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
                                                    <EditField value={row[column.name]} />
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
                                        {
                                            index === indexEdit
                                                ? <>
                                                    <Button icon onClick={() => handleSaveItem(index)}>
                                                        <Icon name="check" />
                                                    </Button>
                                                </>
                                                : <>
                                                    <Button icon onClick={() => handleDeleteItem(index)}>
                                                    <Icon name="delete" />
                                                </Button>
                                                    <Button icon onClick={() => handleEditItem(index)}>
                                                        <Icon name="edit" />
                                                    </Button>
                                                </>
                                        }

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
