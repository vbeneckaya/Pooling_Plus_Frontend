import React from 'react';
import Select from './Select_new';
import {useDispatch, useSelector} from 'react-redux';
import {getLookupRequest, valuesListSelector} from '../../ducks/lookup';
import {columnsSelector} from '../../ducks/dictionaryView';
import {userPermissionsSelector} from '../../ducks/profile';
import Card from '../../containers/customDictionary/card';
import {Button, Icon, Popup} from 'semantic-ui-react';
import {SETTINGS_TYPE_EDIT} from '../../constants/formTypes';
import {useTranslation} from 'react-i18next';

const SoldToField = props => {
    const {value, load, settings, error, textValue, deliveryAddress} = props;
    const dispatch = useDispatch();
    const {t} = useTranslation();
    const valuesList = useSelector(state => valuesListSelector(state, 'soldTo')) || [];
    const soldToItem = valuesList.find(item => item.value === value) || {};
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
        soldToNumber: textValue,
        address: deliveryAddress,
    };
    const userPermissions = useSelector(state => userPermissionsSelector(state));

    const handleLoad = () => {
        dispatch(
            getLookupRequest({
                name: 'soldTo',
                isForm: true,
            }),
        );
        load();
    };

    return (

        <Select {...props}>
            {userPermissions.includes(15) && settings === SETTINGS_TYPE_EDIT ? (
                error ? (
                    <Popup
                        content={t('Добавить в справочник Склады доставки')}
                        position="bottom right"
                        trigger={
                            <Card
                                title={`${t('warehouses')}: ${t('new_record')}`}
                                columns={columns}
                                name="warehouses"
                                defaultForm={defaultForm}
                                loadList={handleLoad}
                            >
                                <Button icon>
                                    <Icon name="add"/>
                                </Button>
                            </Card>
                        }
                    />
                ) : (
                    <Popup
                        content={t('Редактировать данные по складу доставки')}
                        position="bottom right"
                        trigger={
                            <Card
                                title={`${t('warehouses')}: ${t('edit_record')}`}
                                columns={columnsEdit}
                                name="warehouses"
                                id={soldToItem.id}
                                loadList={handleLoad}
                            >
                                <Button icon>
                                    <Icon name="edit"/>
                                </Button>
                            </Card>
                        }
                    />
                )
            ) : null}
        </Select>

    );
};

export default SoldToField;
