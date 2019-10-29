import React, { useEffect, useState, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { Button, Dimmer, Dropdown, Label, Loader, Menu, Popup, Table } from 'semantic-ui-react';
import { gridsMenuSelector } from '../../ducks/profile';
import { getRolesRequest, rolesListSelector } from '../../ducks/roles';
import { getLookupRequest, valuesListSelector } from '../../ducks/lookup';
import { columnsGridSelector } from '../../ducks/gridList';
import './style.scss';
import InfiniteScrollTable from '../../components/InfiniteScrollTable';
import {
    clearFieldsSettings,
    editFieldsSettingRequest,
    editProgressSelector,
    fieldsSettingSelector,
    getFieldsSettingRequest,
    progressSelector,
} from '../../ducks/fieldsSetting';
import { ORDERS_GRID } from '../../constants/grids';

const List = () => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const containerRef = useRef(null);

    const gridsList = useSelector(state => gridsMenuSelector(state)) || [];
    const rolesList = useSelector(state => rolesListSelector(state)) || [];
    const settings = useSelector(state => fieldsSettingSelector(state)) || [];
    const loading = useSelector(state => progressSelector(state));
    const editProgress = useSelector(state => editProgressSelector(state)) || false;

    let [activeItem, setActiveItem] = useState(gridsList[0] || '');
    let [role, setRole] = useState('null');
    let [company, setCompany] = useState('null');

    const statusList =
        useSelector(state =>
            valuesListSelector(state, `${activeItem.slice(0, activeItem.length - 1)}State`),
        ) || [];
    const mainColumnsList = useSelector(state => columnsGridSelector(state, activeItem)) || [];

    useEffect(() => {
        if (!(rolesList || []).length) {
            dispatch(getRolesRequest({}));
        }
    }, []);

    useEffect(
        () => {
            activeItem && getStatus();
        },
        [activeItem],
    );

    useEffect(
        () => {
            dispatch(clearFieldsSettings());
            activeItem && getSettings();
        },
        [role, activeItem],
    );

    const getSettings = () => {
        dispatch(
            getFieldsSettingRequest({
                forEntity: activeItem,
                roleId: role === 'null' ? undefined : role,
            }),
        );
    };

    const getStatus = () => {
        dispatch(
            getLookupRequest({
                name: `${activeItem.slice(0, activeItem.length - 1)}State`,
                isForm: true,
                isSearch: true,
                params: {},
            }),
        );
    };

    const handleChangeActiveItem = (e, { name }) => {
        setActiveItem(name);
    };

    const handleChangSettings = (fieldName, accessType, state = null, isExt) => {
        dispatch(
            editFieldsSettingRequest({
                params: {
                    forEntity: activeItem,
                    roleId: role === 'null' ? undefined : role,
                    fieldName,
                    accessType,
                    state,
                },
                isExt,
                callbackSuccess: () => {
                    getSettings();
                },
            }),
        );
    };

    const rolesListOptions = [
        { key: 'any_role', value: 'null', text: t('any_role') },
        ...rolesList.map(x => ({ key: x.name, value: x.id, text: t(x.name) })),
    ];

    /* const companyListOptions = [
        { key: 'Любая компания', value: 'null', text: t('Любая компания') },
    ];

    const attachmentListOptions = [
        { key: 'Грид', value: 'grid', text: t('Грид') },
        { key: 'Форма', value: 'form', text: t('Форма') },
        { key: 'Форма и грид', value: 'gridAndForm', text: t('Форма и грид') },
    ];*/

    const availabilityListOptions = [
        { key: 'hidden', value: 'hidden', text: t('hidden') },
        { key: 'show', value: 'show', text: t('show') },
        { key: 'edit', value: 'edit', text: t('edit') },
    ];

    const headerRowComponent = (
        <>
            <Table.Row>
                <Table.HeaderCell rowSpan="2">{t('field')}</Table.HeaderCell>
                {/*<Table.HeaderCell rowSpan="2">{t('Принадлежность')}</Table.HeaderCell>*/}
                <Table.HeaderCell colSpan={statusList.length} textAlign="center">
                    {t('status')}
                </Table.HeaderCell>
            </Table.Row>
            <Table.Row className="ext-header-row">
                {statusList.map(status => (
                    <Table.HeaderCell key={status.name} textAlign="center">
                        <Label className="status-label-bottom" color={status.color}>
                            {t(status.name)}
                        </Label>
                    </Table.HeaderCell>
                ))}
            </Table.Row>
        </>
    );

    const FieldRow = ({ field, isExt }) => {
        return (
            <>
                <b>{t(field)}</b>{' '}
                <Popup
                    trigger={<Button size="mini">{t('All')}</Button>}
                    content={
                        <Button.Group>
                            <Button
                                size="mini"
                                onClick={() => {
                                    handleChangSettings(field, 'hidden', null, isExt);
                                }}
                            >
                                {t('hidden')}
                            </Button>
                            <Button
                                size="mini"
                                onClick={() => {
                                    handleChangSettings(field, 'show', null, isExt);
                                }}
                            >
                                {t('show')}
                            </Button>
                            <Button
                                size="mini"
                                onClick={() => {
                                    handleChangSettings(field, 'edit', null, isExt);
                                }}
                            >
                                {t('edit')}
                            </Button>
                        </Button.Group>
                    }
                    on="click"
                    position="top left"
                />
            </>
        );
    };

    const RowBody = ({ column, isExt }) => {
        return (
            <Table.Row key={column.fieldName}>
                <Table.Cell className="table-fields-setting_name">
                    <FieldRow field={column.fieldName} isExt={isExt} />
                </Table.Cell>
                {/* <Table.Cell>
                                    <Dropdown options={attachmentListOptions} />
                                </Table.Cell>*/}
                {statusList.map(status => (
                    <Table.Cell key={`${status.name}_${column.fieldName}`}>
                        <Dropdown
                            options={availabilityListOptions}
                            fluid
                            value={column.accessTypes[status.name]}
                            loading={
                                editProgress &&
                                (editProgress.field === column.fieldName &&
                                    (!editProgress.state || editProgress.state === status.name))
                            }
                            onChange={(e, { value }) =>
                                handleChangSettings(column.fieldName, value, status.name, isExt)
                            }
                        />
                    </Table.Cell>
                ))}
            </Table.Row>
        );
    };

    console.log('settings', statusList);

    const { base: baseSettings = [], ext: extSettings = [] } = settings;

    return (
        <div className="container">
            <Menu>
                {gridsList && gridsList.length
                    ? gridsList.map(item => (
                          <Menu.Item
                              key={item}
                              active={activeItem === item}
                              name={item}
                              onClick={handleChangeActiveItem}
                          >
                              {t(item)}
                          </Menu.Item>
                      ))
                    : null}
                <Menu.Item>
                    <Dropdown
                        value={role}
                        selection
                        options={rolesListOptions}
                        onChange={(e, { value }) => {
                            setRole(value);
                        }}
                    />
                </Menu.Item>
                {/*<Menu.Item>
                    <Dropdown
                        value={company}
                        selection
                        options={companyListOptions}
                        onChange={(e, { value }) => {
                            setCompany(value);
                        }}
                    />
                </Menu.Item>*/}
            </Menu>
            <div className={`scroll-table-container`} ref={containerRef}>
                <InfiniteScrollTable
                    className="grid-table table-info"
                    onBottomVisible={() => {}}
                    structured
                    context={containerRef.current}
                    headerRow={headerRowComponent}
                >
                    <Table.Body className="table-fields-setting">
                        <Dimmer
                            active={loading && !baseSettings.length}
                            inverted
                            className="table-loader table-loader-big"
                        >
                            <Loader size="huge">Loading</Loader>
                        </Dimmer>
                        <Table.Row>
                            <Table.Cell>
                                <div className="ui ribbon label">{t('Main fields')}</div>
                            </Table.Cell>
                            {statusList.map((state, i) => (
                                <Table.Cell key={i} />
                            ))}
                        </Table.Row>
                        {baseSettings.map(column => (
                            <RowBody column={column} />
                        ))}
                        {extSettings.length ? (
                            <>
                                <Table.Row>
                                    <Table.Cell>
                                        <div className="ui ribbon label">{t('articles')}</div>
                                    </Table.Cell>
                                    {/* <Table.Cell />*/}
                                    {statusList.map((state, i) => (
                                        <Table.Cell key={i} />
                                    ))}
                                </Table.Row>
                                {extSettings.map(column => (
                                    <RowBody column={column} isExt/>
                                ))}
                            </>
                        ) : null}
                    </Table.Body>
                </InfiniteScrollTable>
            </div>
        </div>
    );
};

export default List;
