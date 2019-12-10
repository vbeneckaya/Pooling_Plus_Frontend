import React, {useCallback, useEffect, useMemo, useState} from 'react';
import {Confirm, Dimmer, Form, Loader, Grid, Button} from 'semantic-ui-react';
import FormField from '../../components/BaseComponents';
import CardLayout from '../../components/CardLayout';
import {useTranslation} from 'react-i18next';
import {useDispatch, useSelector} from 'react-redux';
import {SELECT_TYPE, TEXT_TYPE} from '../../constants/columnTypes';
import {
    clearUserCard,
    getUserCardRequest,
    progressSelector,
    userCardSelector,
    errorSelector,
    createUserRequest,
} from '../../ducks/users';

const UserCard = props => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const {match, history} = props;
    const {params = {}} = match;
    const {id} = params;

    let [form, setForm] = useState({});
    let [confirmation, setConfirmation] = useState({open: false});
    let [notChangeForm, setNotChangeForm] = useState(true);

    const loading = useSelector(state => progressSelector(state));
    const user = useSelector(state => userCardSelector(state));
    const error = useSelector(state => errorSelector(state)) || {};

    useEffect(() => {
        id && dispatch(getUserCardRequest(id));

        return () => {
            dispatch(clearUserCard());
        };
    }, []);

    useEffect(
        () => {
            setForm(form => ({
                ...form,
                ...user,
            }));
        },
        [user],
    );

    const title = useMemo(
        () => (id ? t('edit_user', {name: user.userName}) : `${t('create_user_title')}`),
        [id, user],
    );

    const getActionsFooter = useCallback(
        () => {
            return (
                <div>
                    <Button color="grey" onClick={handleClose}>
                        {t('CancelButton')}
                    </Button>
                    <Button color="blue" onClick={handleSave}>
                        {t('SaveButton')}
                    </Button>
                </div>
            );
        },
        [form, notChangeForm],
    );

    const handleSave = () => {
        let params = {...form};

        if (id) {
            params = {
                ...params,
                id,
            };
        }

        dispatch(
            createUserRequest({
                params,
                callbackFunc: confirmClose,
            }),
        );
    };

    const handleChange = useCallback(
        (event, {name, value}) => {
            if (notChangeForm) {
                setNotChangeForm(false);
            }
            setForm(form => ({
                ...form,
                [name]: value,
            }));
        },
        [notChangeForm],
    );

    const confirmClose = () => {
        history.goBack();
    };

    const handleClose = () => {
        if (notChangeForm) {
            confirmClose();
        } else {
            setConfirmation({
                open: true,
                content: t('confirm_close_dictionary'),
                onCancel: () => {
                    setConfirmation({
                        confirmation: {open: false},
                    });
                },
                onConfirm: confirmClose,
            });
        }
    };

    const handleRoleChange = () => {
    };

    return (
        <CardLayout title={title} actionsFooter={getActionsFooter} onClose={handleClose}>
            <Dimmer active={loading} inverted>
                <Loader size="huge">Loading</Loader>
            </Dimmer>
            <Form className="user-form">
                <FormField
                    type={TEXT_TYPE}
                    name="email"
                    value={form['email']}
                    isRequired
                    error={error['email']}
                    onChange={handleChange}
                />
                <FormField
                    name="userName"
                    value={form['userName']}
                    type={TEXT_TYPE}
                    isRequired
                    error={error['userName']}
                    onChange={handleChange}
                />
                <FormField
                    typeValue="password"
                    name="password"
                    isRequired={!user.id}
                    value={form['password']}
                    type={TEXT_TYPE}
                    error={error['password']}
                    autoComplete="new-password"
                    onChange={handleChange}
                />
                <FormField
                    fluid
                    search
                    selection
                    text="role"
                    name="roleId"
                    value={form['roleId']}
                    source="roles"
                    isRequired
                    error={error['roleId']}
                    type={SELECT_TYPE}
                    onChange={handleChange}
                />
                <FormField
                    fluid
                    search
                    selection
                    name="carrierId"
                    value={form['carrierId']}
                    source="transportCompanies"
                    error={error['carrierId']}
                    type={SELECT_TYPE}
                    onChange={handleChange}
                />
                {/*{id ? (
                                            <Label pointing>
                                                Оставьте поле пустым, если не хотите менять пароль
                                            </Label>
                                        ) : null}*/}
                <Form.Field>
                    <Form.Checkbox
                        label={t('isActive')}
                        name="isActive"
                        checked={form['isActive']}
                        onChange={(e, {name, checked}) =>
                            handleChange(e, {name, value: checked})
                        }
                    />
                </Form.Field>
            </Form>
            <Confirm
                dimmer="blurring"
                open={confirmation.open}
                onCancel={confirmation.onCancel}
                cancelButton={t('cancelConfirm')}
                onConfirm={confirmation.onConfirm}
                content={confirmation.content}
            />
        </CardLayout>
    );
};

export default UserCard;
