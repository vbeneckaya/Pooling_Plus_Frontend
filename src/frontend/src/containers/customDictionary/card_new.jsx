import React, {useMemo, useCallback, useState, useEffect} from 'react';
import {useTranslation} from 'react-i18next';
import {useDispatch, useSelector} from 'react-redux';
import CardLayout from '../../components/CardLayout';
import {Button, Confirm, Dimmer, Icon, Loader, Popup} from 'semantic-ui-react';
import FormField from '../../components/BaseComponents';
import {
    canDeleteSelector,
    cardProgressSelector,
    cardSelector,
    clearDictionaryCard,
    columnsSelector,
    deleteDictionaryEntryRequest,
    errorSelector,
    getCardRequest,
    saveDictionaryCardRequest,
} from '../../ducks/dictionaryView';

const CardNew = props => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const {match, defaultForm, columns: propsColumns, history, location} = props;
    const {params = {}} = match;
    const {name, id} = params;

    let [form, setForm] = useState({...defaultForm});
    let [confirmation, setConfirmation] = useState({open: false});
    let [notChangeForm, setNotChangeForm] = useState(true);

    const columns = useSelector(
        state => (propsColumns ? propsColumns : columnsSelector(state, name)),
    );
    const canDelete = useSelector(state => canDeleteSelector(state, name));
    const loading = useSelector(state => cardProgressSelector(state));
    const card = useSelector(state => cardSelector(state));
    const error = useSelector(state => errorSelector(state));

    useEffect(() => {
        id && dispatch(getCardRequest({id, name}));

        return () => {
            dispatch(clearDictionaryCard());
        };
    }, []);

    useEffect(
        () => {
            setForm(form => ({
                ...form,
                ...card,
            }));
        },
        [card],
    );

    useEffect(
        () => {
            setForm(form => ({
                ...form,
                ...defaultForm,
            }));
        },
        [defaultForm],
    );

    const title = useMemo(
        () => (id ? `${t(name)}: ${t('edit_record')}` : `${t(name)}: ${t('new_record')}`),
        [name, id],
    );

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
        [form, notChangeForm],
    );

    const handleSave = () => {
        let params = {
            ...form,
        };

        if (id) {
            params = {
                ...params,
                id,
            };
        }

        dispatch(
            saveDictionaryCardRequest({
                params,
                name,
                callbackSuccess: onClose,
            }),
        );
    };

    const handleDelete = () => {
        /* const { id, deleteEntry, name } = this.props;*/

        dispatch(
            deleteDictionaryEntryRequest({
                name,
                id,
                callbackSuccess: onClose,
            }),
        );
    };

    const getActionsHeader = useCallback(() => {
        return (
            <div>
                {canDelete ? (
                    <Popup
                        content={t('delete')}
                        position="bottom right"
                        trigger={
                            <Button icon onClick={handleDelete}>
                                <Icon name="trash alternate outline"/>
                            </Button>
                        }
                    />
                ) : null}
            </div>
        );
    }, []);

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
        setConfirmation({open: false});
    };

    const onClose = () => {
        history.push({
            pathname: location.state.pathname,
            state: {...location.state}
        });
    };

    const handleClose = () => {
        if (notChangeForm) {
            onClose();
        } else {
            setConfirmation({
                open: true,
                content: t('confirm_close_dictionary'),
                onCancel: confirmClose,
                onConfirm: onClose,
            });
        }
    };

    return (
        <CardLayout
            title={title}
            actionsFooter={getActionsFooter}
            actionsHeader={getActionsHeader}
            onClose={handleClose}
            loading={loading}
        >
            <div className="ui form dictionary-edit">
                {columns.map(column => {
                    return (
                        <FormField
                            {...column}
                            noScrollColumn={column}
                            key={column.name}
                            error={error[column.name]}
                            value={form[column.name]}
                            onChange={handleChange}
                        />
                    );
                })}
            </div>
            <Confirm
                dimmer="blurring"
                open={confirmation.open}
                onCancel={confirmation.onCancel}
                cancelButton={t('cancelConfirm')}
                confirmButton={t('Yes')}
                onConfirm={confirmation.onConfirm}
                content={confirmation.content}
            />
            {/*<ConfirmDialog
                open={confirmation.open}
                content={confirmation.content}
                onYesClick={confirmation.onYes}
                onNoClick={confirmation.onNo}
                onCancelClick={confirmation.onCancel}
            />*/}
        </CardLayout>
    );
};

export default CardNew;
