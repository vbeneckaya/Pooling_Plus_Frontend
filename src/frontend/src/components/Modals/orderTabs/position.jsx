import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Button, Form, Grid, Icon, Table } from 'semantic-ui-react';
import Text from '../../BaseComponents/Text';
import { useDispatch, useSelector } from 'react-redux';
import { cardSelector, editCardRequest, settingsExtSelector } from '../../../ducks/gridCard';
import { getLookupRequest, valuesListSelector } from '../../../ducks/lookup';
import Number from '../../BaseComponents/Number';
import { SETTINGS_TYPE_EDIT, SETTINGS_TYPE_SHOW } from '../../../constants/formTypes';
import FormField from '../../BaseComponents';
import { NUMBER_TYPE } from '../../../constants/columnTypes';

const EditField = ({ value, name, onChange, datalist, error, isDisabled }) => {
    return (
        <>
            {name === 'nart' ? (
                <Text
                    value={value}
                    isDisabled={isDisabled}
                    name={name}
                    onChange={onChange}
                    noLabel
                    datalist={datalist}
                />
            ) : (
                <Number
                    value={value}
                    isDisabled={isDisabled}
                    name={name}
                    onChange={onChange}
                    noLabel
                    error={error}
                />
            )}
        </>
    );
};

const Position = ({ form, onChange, gridName, load, settings: baseSettings }) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    let [items, setItems] = useState([...form.items]);
    let [indexEdit, setIndexEdit] = useState(null);
    let [error, setError] = useState(false);

    const card = useSelector(state => cardSelector(state));
    const articles = useSelector(state => valuesListSelector(state, 'articles')) || [];
    const settings = useSelector(state => settingsExtSelector(state, form.status));
    const columns = [];
    Object.keys(settings).forEach(key => {
        if (settings[key] === SETTINGS_TYPE_SHOW || settings[key] === SETTINGS_TYPE_EDIT) {
            columns.push({
                name: key,
            });
        }
    });

    useEffect(() => {
        if (!articles.length) {
            dispatch(
                getLookupRequest({
                    name: 'articles',
                    isForm: true,
                }),
            );
        }
    }, []);

    useEffect(
        () => {
            setItems(form.items);
        },
        [form.items],
    );

    const editPositions = positions => {
        dispatch(
            editCardRequest({
                name: gridName,
                params: {
                    ...card,
                    items: positions,
                },
                callbackSuccess: load,
            }),
        );
    };

    const handleDeleteItem = index => {
        const newItems = items.filter((item, i) => i !== index);

        editPositions(newItems);
    };

    const handleEditItem = index => {
        setIndexEdit(index);
    };

    const handleSaveItem = () => {
        const { quantity } = items[indexEdit];
        if (quantity && parseInt(quantity) >= 0 && !quantity.toString().includes('.')) {
            editPositions(items);
            setIndexEdit(null);
            setError(false);
        } else {
            setError(true);
        }
    };

    const handleChangeField = (e, { name, value }) => {
        const newColumns = [...items];

        newColumns[indexEdit] = {
            ...items[indexEdit],
            [name]: value,
        };

        setItems(newColumns);
    };

    const handleCancelItem = index => {
        setItems([...card.items]);
        setIndexEdit(null);
        setError(false);
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
                            <FormField
                                name="orderAmountExcludingVAT"
                                value={form['orderAmountExcludingVAT']}
                                type={NUMBER_TYPE}
                                settings={baseSettings['orderAmountExcludingVAT']}
                                onChange={onChange}
                            />
                        </Form>
                    </Grid.Column>
                    <Grid.Column className="add-right-elements">
                        <Button disabled={indexEdit !== null} onClick={handleAddItems}>
                            {t('AddButton')}
                        </Button>
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
                            {columns.length && items.length
                                ? items.map((row, index) => (
                                      <Table.Row key={row.id}>
                                          {columns.map(column => (
                                              <>
                                                  {index === indexEdit &&
                                                  (column.name === 'nart' ||
                                                      column.name === 'quantity') ? (
                                                      <Table.Cell
                                                          key={`cell_${row.id}_${
                                                              column.name
                                                          }_${index}`}
                                                          className={`table-edit-field-${
                                                              column.name
                                                          }`}
                                                      >
                                                          <EditField
                                                              value={row[column.name]}
                                                              name={column.name}
                                                              isDisabled={
                                                                  settings[column.name] ===
                                                                  SETTINGS_TYPE_SHOW
                                                              }
                                                              datalist={
                                                                  column.name === 'nart' && articles
                                                              }
                                                              error={error}
                                                              onChange={handleChangeField}
                                                          />
                                                      </Table.Cell>
                                                  ) : (
                                                      <Table.Cell
                                                          key={`cell_${row.id}_${
                                                              column.name
                                                          }_${index}`}
                                                      >
                                                          {row[column.name]}
                                                      </Table.Cell>
                                                  )}
                                              </>
                                          ))}
                                          <Table.Cell textAlign="right">
                                              {index === indexEdit ? (
                                                  <>
                                                      <Button
                                                          icon
                                                          onClick={() => handleCancelItem(index)}
                                                      >
                                                          <Icon name="undo alternate" />
                                                      </Button>
                                                      <Button
                                                          icon
                                                          onClick={() => handleSaveItem(index)}
                                                      >
                                                          <Icon name="check" />
                                                      </Button>
                                                  </>
                                              ) : (
                                                  <>
                                                      <Button
                                                          disabled={indexEdit !== null}
                                                          icon
                                                          onClick={() => handleEditItem(index)}
                                                      >
                                                          <Icon name="pencil alternate" />
                                                      </Button>
                                                      <Button
                                                          disabled={indexEdit !== null}
                                                          icon
                                                          onClick={() => handleDeleteItem(index)}
                                                      >
                                                          <Icon name="trash alternate" />
                                                      </Button>
                                                  </>
                                              )}
                                          </Table.Cell>
                                      </Table.Row>
                                  ))
                                : null}
                        </Table.Body>
                    </Table>
                </Grid.Row>
            </Grid>
        </>
    );
};

export default Position;
