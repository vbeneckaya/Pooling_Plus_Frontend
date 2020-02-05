import React, { useMemo, useCallback, useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import CardLayout from '../../components/CardLayout';
import { Button, Confirm, Dimmer, Icon, Loader, Modal, Popup } from 'semantic-ui-react';
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
    progressSelector,
    saveDictionaryCardRequest,
} from '../../ducks/dictionaryView';

const Content = ({ columns, error, form, handleChange }) => {
    return (
        <div className="ui form dictionary-edit">
            {columns.map(column => {
                if (column.name != 'isActive')
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
    );
};

const CardNew = props => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const {
        match = {},
        defaultForm,
        columns: propsColumns,
        history,
        location,
        load,
        isModal,
        openModal,
        onClose: onCloseModal,
    } = props;
    const { params = {} } = match;
    const { name, id } = params;

    let [form, setForm] = useState({ ...defaultForm });
    let [confirmation, setConfirmation] = useState({ open: false });
    let [notChangeForm, setNotChangeForm] = useState(true);

    const columns = useSelector(
        state => (propsColumns ? propsColumns : columnsSelector(state, name)),
    );
    const canDelete = useSelector(state => canDeleteSelector(state, name));
    const loading = useSelector(state => cardProgressSelector(state));
    const progress = useSelector(state => progressSelector(state));
    const card = useSelector(state => cardSelector(state));
    const error = useSelector(state => errorSelector(state));

    useEffect(() => {
      //  console.log('454545');
        id && dispatch(getCardRequest({ id, name }));

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

    const onOpenModal = () => {
     //   console.log('ooopen');
    };

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
                    <Button
                        color="blue"
                        disabled={notChangeForm}
                        loading={progress}
                        onClick={handleSave}
                    >
                        {t('SaveButton')}
                    </Button>
                </>
            );
        },
        [form, notChangeForm, progress],
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
                callbackSuccess: () => {
                    load && load(form);
                    onClose();
                },
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
                                <Icon name="trash alternate outline" />
                            </Button>
                        }
                    />
                ) : null}
            </div>
        );
    }, []);

    const handleChange = useCallback(
        (event, { name, value }) => {
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
        setConfirmation({ open: false });
    };

    const onClose = () => {
        isModal
            ? onCloseModal()
            : history.push({
                  pathname: location.state.pathname,
                  state: { ...location.state },
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
        <>
            {isModal ? (
                <Modal
                    dimmer="blurring"
                    open={openModal}
                    closeOnDimmerClick={false}
                    onOpen={onOpenModal}
                    onClose={onCloseModal}
                    closeIcon
                >
                    <Modal.Header>{title}</Modal.Header>
                    <Modal.Description>
                        {/*<Loader size="huge" active={loading}>
                            Loading
                        </Loader>*/}
                        <Content
                            columns={columns}
                            error={error}
                            form={form}
                            handleChange={handleChange}
                        />
                    </Modal.Description>
                    <Modal.Actions>{getActionsFooter()}</Modal.Actions>
                </Modal>
            ) : (
                <CardLayout
                    title={title}
                    actionsFooter={getActionsFooter}
                    actionsHeader={getActionsHeader}
                    onClose={handleClose}
                    loading={loading}
                >
                    <Content
                        columns={columns}
                        error={error}
                        form={form}
                        handleChange={handleChange}
                    />
                </CardLayout>
            )}
            <Confirm
                dimmer="blurring"
                open={confirmation.open}
                onCancel={confirmation.onCancel}
                cancelButton={t('cancelConfirm')}
                confirmButton={t('Yes')}
                onConfirm={confirmation.onConfirm}
                content={confirmation.content}
            />
        </>
    );
};

export default CardNew;
