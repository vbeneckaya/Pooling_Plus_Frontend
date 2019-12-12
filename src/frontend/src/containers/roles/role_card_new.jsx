import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import {
    allActionsSelector,
    allPermissionsSelector,
    clearRolesCard,
    createRoleRequest,
    errorSelector,
    getAllActionsRequest,
    getAllPermissionsRequest,
    getRoleCardRequest,
    progressSelector,
    roleCardSelector,
} from '../../ducks/roles';
import CardLayout from '../../components/CardLayout';
import {Button, Dimmer, Form, Loader, Tab} from 'semantic-ui-react';
import FormField from '../../components/BaseComponents';
import {sortFunc} from '../../utils/sort';
import {TEXT_TYPE} from '../../constants/columnTypes';

const RoleCard = props => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const { match, history, location } = props;
    const { params = {} } = match;
    const { id } = params;

    let [form, setForm] = useState({});

    const loading = useSelector(state => progressSelector(state));
    const role = useSelector(state => roleCardSelector(state)) || {};
    const error = useSelector(state => errorSelector(state)) || {};
    const allPermissions = useSelector(state => allPermissionsSelector(state)) || [];
    const allActions = useSelector(state => allActionsSelector(state)) || [];

    useEffect(() => {
        id && dispatch(getRoleCardRequest(id));
        dispatch(getAllPermissionsRequest());
        dispatch(getAllActionsRequest());

        return () => {
            dispatch(clearRolesCard());
        };
    }, []);

    useEffect(
        () => {
            setForm(form => ({
                ...form,
                ...role,
                permissions: role.permissions ? role.permissions.map(item => item.code) : [],
            }));
        },
        [role],
    );

    const title = useMemo(
        () => (id ? t('edit_role', { name: role.name }) : `${t('create_role_title')}`),
        [id, role],
    );

    const handleClose = () => {
        history.push({
            pathname: location.state.pathname,
            state: {...location.state}
        });
    };

    const handleChange = useCallback((e, { name, value }) => {
        setForm(form => ({
            ...form,
            [name]: value,
        }));
    }, []);

    const handlePermissions = (e, { value }) => {
        const { permissions } = form;

        const selectedPermissions = new Set(permissions);

        selectedPermissions[selectedPermissions.has(value) ? 'delete' : 'add'](value);

        if (value === 1 && !selectedPermissions.has(value)) {
            selectedPermissions.delete(2);
            selectedPermissions.delete(4);
            selectedPermissions.delete(5);
            selectedPermissions.delete(6);
        }

        if (value === 7 && !selectedPermissions.has(value)) {
            selectedPermissions.delete(10);
            selectedPermissions.delete(11);
            selectedPermissions.delete(12);
        }

        handleChange(null, { name: 'permissions', value: Array.from(selectedPermissions) });
    };

    const handleActions = (e, {value}) => {
        const {actions} = form;

        const selectedActions = new Set(actions);

        selectedActions[selectedActions.has(value) ? 'delete' : 'add'](value);

        handleChange(null, {name: 'actions', value: Array.from(selectedActions)});
    };

    const mapData = () => {
        return {
            ...form,
            permissions: form.permissions.map(item => ({
                code: item,
            })),
        };
    };

    const handleSave = () => {
        dispatch(createRoleRequest({params: mapData(), callbackFunc: handleClose}));
    };

    const getActionsFooter = useCallback(
        () => {
            return (
                <>
                    <Button color="grey" onClick={handleClose}>
                        {t('CancelButton')}
                    </Button>
                    <Button color="blue" onClick={handleSave}>
                        {t('SaveButton')}
                    </Button>
                </>
            );
        },
        [form],
    );

    const getContent = useCallback(() => {
        return [
            {
                menuItem: 'general',
                render: () => (
                    <Form className="role-form">
                        <FormField
                            name="name"
                            value={form['name']}
                            type={TEXT_TYPE}
                            isRequired
                            error={error['name']}
                            onChange={handleChange}
                        />
                        <Form.Field>
                            <label>{t('permissions')}</label>
                        </Form.Field>
                        {allPermissions.map(permission => (
                            <Form.Field key={permission.code}>
                                <Form.Checkbox
                                    label={t(permission.name)}
                                    value={permission.code}
                                    checked={
                                        permissions && permissions.includes(permission.code)
                                    }
                                    disabled={
                                        (permissions &&
                                            ([2, 4, 5, 6].includes(permission.code) &&
                                                !permissions.includes(1))) ||
                                        ([10, 11, 12].includes(permission.code) &&
                                            !permissions.includes(7))
                                    }
                                    onChange={handlePermissions}
                                />
                            </Form.Field>
                        ))}
                    </Form>
                ),
            },
            {
                menuItem: 'order_actions',
                render: () => (
                    <Form className="role-form">
                        {sortFunc(orderActions, t).map(action => (
                            <Form.Field key={action}>
                                <Form.Checkbox
                                    label={t(action)}
                                    value={action}
                                    checked={actions && actions.includes(action)}
                                    onChange={handleActions}
                                />
                            </Form.Field>
                        ))}
                    </Form>
                ),
            },
            {
                menuItem: 'shipping_actions',
                render: () => (
                    <Form className="role-form">
                        {sortFunc(shippingActions, t).map(action => (
                            <Form.Field key={action}>
                                <Form.Checkbox
                                    label={t(action)}
                                    value={action}
                                    checked={actions && actions.includes(action)}
                                    onChange={handleActions}
                                />
                            </Form.Field>
                        ))}
                    </Form>
                ),
            },
        ];
    }, [form, error]);

    const permissions = useMemo(() => form.permissions || [], [form]);
    const actions = useMemo(() => form.actions || [], [form]);
    const orderActions = useMemo(() => allActions.orderActions || [], [allActions]);
    const shippingActions = useMemo(() => allActions.shippingActions || [], [allActions]);

    return (
        <CardLayout
            title={title}
            actionsFooter={getActionsFooter}
            content={getContent}
            onClose={handleClose}
            loading={loading}
        />
    );
};

export default RoleCard;
