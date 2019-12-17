import React, { useEffect } from 'react';
import Select from './Select_new';
import { useDispatch, useSelector } from 'react-redux';
import { getLookupRequest, valuesListSelector } from '../../ducks/lookup';
import { columnsSelector } from '../../ducks/dictionaryView';
import { userPermissionsSelector } from '../../ducks/profile';
import Card from '../../containers/customDictionary/card';
import { Button, Icon, Popup } from 'semantic-ui-react';
import { SETTINGS_TYPE_EDIT } from '../../constants/formTypes';
import { useTranslation } from 'react-i18next';
import { addError, clearError } from '../../ducks/gridCard';

const SoldToField = props => {
    const { value, settings, error, textValue, deliveryAddress, onChange, name } = props;
    const dispatch = useDispatch();
    const { t } = useTranslation();

    const valuesList = useSelector(state => valuesListSelector(state, 'soldTo')) || [];
    const soldToItem = value ? valuesList.find(item => item.value === value.value) || {} : {};
    const columns = useSelector(state => columnsSelector(state, 'warehouses')) || [];
    const columnsEdit = columns.map(item => {
        if (item.name === 'soldToNumber') {
            return {
                ...item,
                isDisabled: true,
            };
        } else return item;
    });
    const defaultForm = {
        soldToNumber: textValue ? textValue.name : null,
        address: deliveryAddress,
    };
    const userPermissions = useSelector(state => userPermissionsSelector(state));

    const handleLoad = form => {
        dispatch(
            getLookupRequest({
                name: 'soldTo',
                isForm: true,
                callbackSuccess: result => {
                    onChange(null, {
                        value: {
                            ...form,
                            value: form.soldToNumber,
                            name: form.soldToNumber
                        },
                        name,
                    });
                },
            }),
        );
    };

    useEffect(() => {
        dispatch(
            getLookupRequest({
                name: 'soldTo',
                isForm: true,
            }),
        );
    }, []);

    useEffect(() => {
        if (value && Object.keys(value).length && valuesList.length && !valuesList.find(item => item.value === value.value)) {
            dispatch(
                addError({
                    name: 'soldTo',
                    message: t('soldTo_error'),
                }),
            );
        } else if (error) {
            dispatch(clearError('soldTo'));
        }
    }, [valuesList, value]);

    return (
        <Select {...props}>
            {userPermissions.includes(15) && settings === SETTINGS_TYPE_EDIT ? (
                error ? (
                    <Popup
                        content={t('Add to the catalog Delivery warehouses')}
                        position="bottom right"
                        trigger={
                            <div>
                                <Card
                                    title={`${t('warehouses')}: ${t('new_record')}`}
                                    columns={columns}
                                    name="warehouses"
                                    defaultForm={defaultForm}
                                    load={handleLoad}
                                >
                                    <Button icon>
                                        <Icon name="add" />
                                    </Button>
                                </Card>
                            </div>
                        }
                    />
                ) : (
                    <Popup
                        content={t('Edit delivery warehouse data')}
                        position="bottom right"
                        trigger={
                            <div>
                                <Card
                                    title={`${t('warehouses')}: ${t('edit_record')}`}
                                    columns={columnsEdit}
                                    name="warehouses"
                                    id={soldToItem.id}
                                    load={handleLoad}
                                >
                                    <Button icon>
                                        <Icon name="edit" />
                                    </Button>
                                </Card>
                            </div>
                        }
                    />
                )
            ) : null}
        </Select>
    );
};

export default SoldToField;
