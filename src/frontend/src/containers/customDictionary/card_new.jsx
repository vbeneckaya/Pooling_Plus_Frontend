import React, {useMemo, useCallback, useState, useEffect} from 'react';
import {useTranslation} from 'react-i18next';
import {useDispatch, useSelector} from 'react-redux';
import CardLayout from '../../components/CardLayout';
import {Button, Icon, Popup} from 'semantic-ui-react';
import FormField from '../../components/BaseComponents';
import {
    canDeleteSelector,
    cardProgressSelector,
    cardSelector,
    columnsSelector,
    errorSelector,
    getCardRequest,
} from '../../ducks/dictionaryView';

const CardNew = props => {
    console.log('props', props);
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const {match, defaultForm, columns: propsColumns} = props;
    const {params = {}} = match;
    const {name, id} = params;

    let [form, setForm] = useState({...defaultForm});

    const columns = useSelector(
        state => (propsColumns ? propsColumns : columnsSelector(state, name)),
    );
    const canDelete = useSelector(state => canDeleteSelector(state, name));
    const loading = useSelector(state => cardProgressSelector(state));
    const card = useSelector(state => cardSelector(state));
    const error = useSelector(state => errorSelector(state));

    useEffect(() => {
        id && dispatch(getCardRequest({id, name}));
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

    const getActionsFooter = useCallback(() => {
        return (
            <div>
                <Button color="grey">{t('CancelButton')}</Button>
                <Button color="blue">{t('SaveButton')}</Button>
            </div>
        );
    }, []);

    const getActionsHeader = useCallback(() => {
        return (
            <div>
                {canDelete ?
                    <Popup
                        content={t('delete')}
                        position="bottom right"
                        trigger={
                            <Button icon>
                                <Icon name="trash alternate outline"/>
                            </Button>
                        }
                    />
                    : null}
            </div>
        );
    }, []);

    const handleChange = useCallback((event, {name, value}) => {
        setForm(form => ({
            ...form,
            [name]: value,
        }));
    }, []);

    return (
        <CardLayout title={title} actionsFooter={getActionsFooter} actionsHeader={getActionsHeader}>
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
        </CardLayout>
    );
};

export default CardNew;
