import React, { useEffect, useState, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { Button, Dropdown, Label, Menu, Popup, Table } from 'semantic-ui-react';
import { gridsMenuSelector } from '../../ducks/profile';
import { getRolesRequest, rolesListSelector } from '../../ducks/roles';
import { getLookupRequest, valuesListSelector } from '../../ducks/lookup';
import { columnsGridSelector } from '../../ducks/gridList';
import './style.scss';
import InfiniteScrollTable from '../../components/InfiniteScrollTable';

const List = () => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const containerRef = useRef(null);

    const gridsList = useSelector(state => gridsMenuSelector(state)) || [];
    const rolesList = useSelector(state => rolesListSelector(state)) || [];

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

    useEffect(() => {
        activeItem && getStatus();
    }, [activeItem]);

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

    const rolesListOptions = [
        { key: 'Любая роль', value: 'null', text: t('Любая роль') },
        ...rolesList.map(x => ({ key: x.name, value: x.id, text: t(x.name) })),
    ];

    const companyListOptions = [
        { key: 'Любая компания', value: 'null', text: t('Любая компания') },
    ];

    const attachmentListOptions = [
        { key: 'Грид', value: 'grid', text: t('Грид') },
        { key: 'Форма', value: 'form', text: t('Форма') },
        { key: 'Форма и грид', value: 'gridAndForm', text: t('Форма и грид') },
    ];

    const availabilityListOptions = [
        { key: 'Скрыто', value: 'hidden', text: t('Скрыто') },
        { key: 'Просмотр', value: 'view', text: t('Просмотр') },
        { key: 'Редактирование', value: 'edit', text: t('Редактирование') },
    ];

    const headerRowComponent = (
        <>
            <Table.Row>
                <Table.HeaderCell rowSpan="2">{t('Поле')}</Table.HeaderCell>
                <Table.HeaderCell rowSpan="2">{t('Принадлежность')}</Table.HeaderCell>
                <Table.HeaderCell colSpan={statusList.length} textAlign="center">
                    {t('Статус')}
                </Table.HeaderCell>
            </Table.Row>
            <Table.Row className="ext-header-row">
                {statusList.map(status => (
                    <Table.HeaderCell key={status.name}>
                        <Label className="status-label-bottom" color={status.color}>
                            {t(status.name)}
                        </Label>
                    </Table.HeaderCell>
                ))}
            </Table.Row>
        </>
    );

    const FieldRow = ({field}) => {
        return (
            <>
                <b>{t(field)}</b>{' '}
                <Popup
                    trigger={<Button size="mini">All</Button>}
                    content={
                        <Button.Group>
                            <Button size="mini" onClick={() => {}}>
                                Hide
                            </Button>
                            <Button size="mini" onClick={() => {}}>
                                Show
                            </Button>
                            <Button size="mini" onClick={() => {}}>
                                Edit
                            </Button>
                        </Button.Group>
                    }
                    on="click"
                    position="top left"
                />
            </>
        );
    };

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
                <Menu.Item>
                    <Dropdown
                        value={company}
                        selection
                        options={companyListOptions}
                        onChange={(e, { value }) => {
                            setCompany(value);
                        }}
                    />
                </Menu.Item>
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
                        <Table.Row>
                            <Table.Cell>
                                <div className="ui ribbon label">{t('Main fields')}</div>
                            </Table.Cell>
                            <Table.Cell/>
                            {statusList.map((state, i) => <Table.Cell key={i} />)}
                        </Table.Row>
                        {mainColumnsList.map(column => (
                            <Table.Row key={column.name}>
                                <Table.Cell><FieldRow field={column.name} /></Table.Cell>
                                <Table.Cell>
                                    <Dropdown options={attachmentListOptions} />
                                </Table.Cell>
                                {statusList.map(status => (
                                    <Table.Cell>
                                        <Dropdown options={availabilityListOptions} />
                                    </Table.Cell>
                                ))}
                            </Table.Row>
                        ))}
                        <Table.Row>
                            <Table.Cell>
                                <div className="ui ribbon label">{t('articles')}</div>
                            </Table.Cell>
                            <Table.Cell />
                            {statusList.map((state, i) => <Table.Cell key={i} />)}
                        </Table.Row>
                    </Table.Body>
                </InfiniteScrollTable>
            </div>
        </div>
    );
};

export default List;
