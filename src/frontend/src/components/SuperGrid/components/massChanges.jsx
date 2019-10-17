import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector, useDispatch } from 'react-redux';
import { Button, Dropdown, Form, Grid, Icon } from 'semantic-ui-react';
import {invokeMassUpdateRequest, updatesSelector} from '../../../ducks/gridActions';
import FormField from '../../BaseComponents';

const MassChanges = ({gridName, load}) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const updates = useSelector(state => updatesSelector(state)) || [];
    let [field, setField] = useState(null);
    let [column, setColumn] = useState({});
    let [changValue, setValue] = useState(null);

    const fieldParams = updates.find(item => item.name === field);

    useEffect(
        () => {
            console.log('gg', fieldParams);
            setColumn({
                name: field,
                type: fieldParams && fieldParams.type,
                noLabel: true,
                placeholder: t('new_value')
            });
        },
        [field],
    );

    const handleSave = () => {
        dispatch(invokeMassUpdateRequest({
            name: gridName,
            field,
            ids: fieldParams.ids,
            value: changValue,
            callbackSuccess: load
        }))
    };

    return (
        <Form className="grid-mass-updates">
            {updates.length ? (
                <>
                    <label>{t('change')}</label>
                    <Dropdown
                        placeholder={t('choose_option')}
                        fluid
                        selection
                        value={field}
                        options={updates.map(item => ({
                            key: item.name,
                            value: item.name,
                            text: t(item.name),
                        }))}
                        style={{ width: '40%' }}
                        onChange={(e, { value }) => setField(value)}
                    />
                    {column.type ? (
                        <>
                            <FormField
                                column={column}
                                value={changValue}
                                onChange={(e, { name, value }) => setValue(value)}
                            />
                            <Button icon disabled={!changValue} onClick={handleSave}>
                                <Icon name="save" />
                            </Button>
                        </>
                    ) : null}
                </>
            ) : null}
        </Form>
    );
};

export default MassChanges;
