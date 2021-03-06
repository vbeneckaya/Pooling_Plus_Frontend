import React, {useCallback, useEffect, useMemo, useState} from 'react';
import {useDispatch, useSelector} from 'react-redux';
import {useTranslation} from 'react-i18next';

import {Button, Icon, Popup} from 'semantic-ui-react';
import {
    cardSelector,
    clearGridCard,
    editCardRequest,
    editProgressSelector,
    errorSelector,
    getCardRequest,
    isUniqueNumberRequest,
    progressSelector,
    settingsFormSelector,
    settingsFormExtSelector,
} from '../../ducks/gridCard';
import {
    actionsCardSelector,
    clearActions,
    getActionsRequest,
    invokeActionRequest,
    progressActionNameSelector,
} from '../../ducks/gridActions';
import {ORDERS_GRID, SHIPPINGS_GRID} from '../../constants/grids';
import OrderCard from './components/orderCard';
import ShippingCard from './components/shippingCard';
import {GRID_CARD_LINK} from '../../router/links';
import {clearHistory, getHistoryRequest} from '../../ducks/history';
import {columnsGridSelector} from "../../ducks/gridList";
import {BIG_TEXT_TYPE, NUMBER_TYPE, TEXT_TYPE} from "../../constants/columnTypes";

const Card = props => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const {match, history, location} = props;
    const {params = {}} = match;
    const {name, id} = params;

    let [form, setForm] = useState({});
    let [justOpen, setJustOpen] = useState(true)
    let [allowToSend, setAllowToSend] = useState(false);


    const card = useSelector(state => cardSelector(state));
    const settings = useSelector(state => settingsFormSelector(state, card.status));
    const error = useSelector(state => errorSelector(state));
    const columns = useSelector(state => columnsGridSelector(state, name));

    const title = useMemo(
        () =>
            id
                ? t(`edit_${name}`, {
                    number: name === ORDERS_GRID ? (!card.orderNumber ? card.orderNumber : card.orderNumber.value) :
                        (!!card.shippingNumber ? card.shippingNumber.value : card.shippingNumber),
                    status: t(form.status),
                })
                : t(`new_${name}`),
        [name, id, form],
    );

    useEffect(() => {
        setJustOpen(true);
        dispatch(clearActions());
        id && loadCard();

        return () => {
            dispatch(clearHistory());
        };
    }, []);

    useEffect(
        () => {
            if (allowToSend) {
                handleSave();
            }
        },
        [form],
    );

    const updateCard =() => {
        setJustOpen(false);
        loadCard();
    }
    
    const loadCard = () => {
        id && id != 'new' &&
        dispatch(
            getCardRequest({
                name,
                id,
                callbackSuccess: card => {
                    setForm(card);
                },
            }),
        );

        id && id != 'new' &&
        dispatch(
            getActionsRequest({
                name,
                ids: [id],
                isCard: true,
            }),
        );
        id && id != 'new' && dispatch(getHistoryRequest(id));

       // id && id == 'new' && setForm(card);
    };

    const onClose = () => {
        const {state} = location;
        if (!!state) {
            const {pathname, gridLocation} = state;
            history.replace({
                pathname: pathname,
                state: {
                    ...state,
                    pathname: gridLocation,
                },
            });
        }
    };

    const handleClose = () => {
        dispatch(clearGridCard());
        onClose();
    };

    const onChangeForm = useCallback((e, {name, value}) => {
        switch (name) {
            case 'clientId':
                setForm(prevState => ({
                    ...prevState,
                    [name]: value,
                    ['deliveryWarehouseId']: null
                }));
                break;
            case 'orderNumber':
            case 'shippingNumber':
                setForm(prevState => ({
                    ...prevState,
                    [name]: {value: value, name: null},
                }));
                break;
            default:
                setForm(prevState => ({
                    ...prevState,
                    [name]: value,
                }));
        }

        let column = columns.filter(_ => _.name == name)[0];
        let columnType = name == 'orderNumber' || name == 'shippingNumber' ? TEXT_TYPE : column && column.type;

        switch (columnType) {
            case TEXT_TYPE:
            case BIG_TEXT_TYPE:
            case NUMBER_TYPE:
                setAllowToSend(false);
                break;
            default:
                setAllowToSend(true);
        }
    }, []);

    const onBlurForm = () => {
        handleSave();
    };


    const saveOrEditForm = () => {
        setJustOpen(false);
        dispatch(
            editCardRequest({
                name,
                params: form,
                callbackSuccess: (result) => {
                    setAllowToSend(false);
                     if (!!form.id) {
                        loadCard();
                    }
                }
            }),
        );
    };

    const handleSave = () => {
        if (name === ORDERS_GRID) {
            handleUniquenessCheck(saveOrEditForm);
        } else {
            saveOrEditForm();
        }
    };


    const invokeAction = (actionName, itemId) => {
        dispatch(
            invokeActionRequest({
                ids: [!!itemId ? itemId : id],
                name,
                actionName,
                callbackSuccess: () => {
                    if (actionName.toLowerCase().includes('delete')) {
                        onClose();
                    }
                    
                     if (id != 'new' && !actionName.toLowerCase().includes('delete')){
                        loadCard();
                    }
                },
            }),
        );
    };

    const handleUniquenessCheck = callbackFunc => {
        if (form.orderNumber && card.orderNumber && (!id || form.orderNumber.value !== card.orderNumber.value)) {
            dispatch(
                isUniqueNumberRequest({
                    number: form.orderNumber ? form.orderNumber.value : null,
                    providerId: form.providerId ? form.providerId.value : null,
                    fieldName: 'orderNumber',
                    errorText: t('number_already_exists'),
                    callbackSuccess: callbackFunc,
                }),
            );
        }
        else callbackFunc();
    };

    const loading = useSelector(state => progressSelector(state));
  //  const editLoading = useSelector(state => editProgressSelector(state));
    const actions = useSelector(state => actionsCardSelector(state));
  //  const progressActionName = useSelector(state => progressActionNameSelector(state));

    const goToCard = (gridName, cardId) => {
        const {state} = location;
            history.replace({
                pathname: GRID_CARD_LINK.replace(':name', gridName).replace(':id', cardId),
                state: {
                    ...state,
                    pathname: history.location.pathname,
                    gridLocation: state && state.gridLocation ? state.gridLocation : state && state.pathname
                },
            });
    };

    const getActionsHeader = useCallback(
        () => {
            return (
                <div className="grid-card-header">
                    {name === ORDERS_GRID && form.shippingId ? (
                        <div
                            className="link-cell"
                            onClick={() => goToCard(SHIPPINGS_GRID, form.shippingId)}
                        >
                            {t('open_shipping', {number: form.shippingNumber.value})}
                        </div>
                    ) : null}
                    {actions &&
                    actions.filter(item => item.allowedFromForm).map(action => (
                        <Popup
                            content={action.description}
                            disabled={!action.description}
                            trigger={
                                <Button
                                    className="grid-card-header_actions_button"
                                    key={action.name}
                                    loading={action.loading}
                                    disabled={action.loading}
                                    size="mini"
                                    compact
                                    onClick={() => invokeAction(action.name)}
                                >
                                    <Icon name="circle" color={action.color}/>
                                    {t(action.name)}
                                </Button>
                            }
                        />
                    ))}
                </div>
            );
        },
        [form, actions, name],
    );

    return (
        <React.Fragment>
            {name === ORDERS_GRID ? (
                <OrderCard
                    {...props}
                    id={id}
                    load={loadCard}
                    update={updateCard}
                    name={name}
                    form={form}
                    title={title}
                    settings={settings}
                    loading={loading && justOpen}
                    uniquenessNumberCheck={handleSave} //{handleUniquenessCheck}
                    error={error}
                    onClose={handleClose}
                    onChangeForm={onChangeForm}
                    onBlurForm={onBlurForm}
                    actionsHeader={(!!id && id != 'new') ? getActionsHeader : null}
                />
            ) : (
                <ShippingCard
                    {...props}
                    title={title}
                    id={id}
                    name={name}
                    form={form}
                    load={loadCard}
                    update={updateCard}
                    goToCard={goToCard}
                    loading={loading && justOpen}
                    settings={settings}
                    error={error}
                    onClose={handleClose}
                    onBlurForm={onBlurForm}
                    onChangeForm={onChangeForm}
                    actionsHeader={getActionsHeader}
                />
            )}
        </React.Fragment>
    );
};

export default Card;
