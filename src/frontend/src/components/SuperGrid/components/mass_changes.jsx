import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { Button, Dropdown, Form, Grid, Icon } from 'semantic-ui-react';
import { invokeMassUpdateRequest, updatesSelector } from '../../../ducks/gridActions';
import FormField from '../../BaseComponents';
import { TEXT_TYPE } from '../../../constants/columnTypes';

const Mass_changes = ({ gridName, load }) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const updates = useSelector(state => updatesSelector(state)) || [];
    let [field, setField] = useState(null);
    let [column, setColumn] = useState({});
    let [changValue, setValue] = useState(null);

    const fieldParams = updates.find(item => item.name === field);

    useEffect(
        () => {
            setColumn({
                name: field,
                type: fieldParams ? fieldParams.type : TEXT_TYPE,
                noLabel: true,
                isDisabled: !field,
                placeholder: t('new_value'),
            });
        },
        [field],
    );

    const handleSave = () => {
        dispatch(
            invokeMassUpdateRequest({
                name: gridName,
                field,
                ids: fieldParams.ids,
                value: changValue,
                callbackSuccess: load,
            }),
        );
    };

    return (
        <Form className="grid-mass-updates">
            {updates.length ? (
                <Grid>
                    <Grid.Row columns="equal">
                        <Grid.Column className="grid-mass-updates-fields">
                            <label>{t('change')}</label>
                            <Dropdown
                                placeholder={t('choose_option')}
                                fluid
                                selection
                                value={field}
                                upward
                                options={updates.map(item => ({
                                    key: item.name,
                                    value: item.name,
                                    text: t(item.name),
                                }))}
                                onChange={(e, { value }) => setField(value)}
                            />
                        </Grid.Column>
                        <Grid.Column className="grid-mass-updates-fields">
                            <FormField
                                column={column}
                                value={changValue}
                                onChange={(e, { name, value }) => setValue(value)}
                            />
                            <Button
                                icon
                                disabled={!changValue}
                                className="grid-mass-updates-save"
                                onClick={handleSave}
                            >
                                <Icon name="save" />
                            </Button>
                        </Grid.Column>
                    </Grid.Row>
                </Grid>
            ) : null}
        </Form>
    );
};

export default Mass_changes;
