import React, {useState, useEffect, useRef} from 'react';
import {useTranslation} from 'react-i18next';
import {useDispatch, useSelector} from 'react-redux';

import {Dropdown, Form, Visibility} from 'semantic-ui-react';

import './style.scss';
import {
    getLookupRequest,
    listFromSelectSelector,
    progressSelector,
    valuesListSelector,
} from '../../ducks/lookup';
import {PAGE_SIZE} from '../../constants/settings';
import {debounce} from 'throttle-debounce';

const Select = ({
                    value,
                    onChange,
                    placeholder = '',
                    isDisabled,
                    label,
                    name,
                    text,
                    multiple,
                    loading,
                    clearable,
                    source,
                    isTranslate,
                    error,
                    textValue,
                    noLabel,
                    isRequired,
                    autoComplete,
                    children,
                }) => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const context = useRef(null);
    let timer = useRef(null);

    let [open, setOpen] = useState(false);
    let [items, setItems] = useState([]);
    let [counter, setCounter] = useState(PAGE_SIZE);
    let [searchQuery, setSearchQuery] = useState(null);
    let [filter, setFilter] = useState('');

    const valuesList =
        useSelector(state => listFromSelectSelector(state, source, t, filter, isTranslate)) || [];
    const progress = useSelector(state => progressSelector(state));

    const changeItems = () => {
        setItems(valuesList.slice(0, counter));
    };

    useEffect(
        () => {
            changeItems();
        },
        [valuesList, counter],
    );

    useEffect(
        () => {
            clearTimeout(timer.current);
            timer.current = setTimeout(() => {
                console.log('%%%%%');
                setFilter(searchQuery);
            }, 300);
        },
        [searchQuery],
    );

    useEffect(
        () => {
            context.current.scrollTop = 0;
            setCounter(PAGE_SIZE);
        },
        [filter],
    );

    const handleChange = (e, {value}) => {
        console.log(value, valuesList.find(x => x.value === value));
        setSearchQuery('');
        toggle(false);
        onChange(e, {value, name, ext: valuesList.find(x => x.value === value)});
    };

    const handleOpen = () => {
        toggle(true);
        dispatch(
            getLookupRequest({
                name: source,
                isForm: true,
            }),
        );
    };

    const handleClose = () => {
        context.current.scrollTop = 0;
        setCounter(PAGE_SIZE);
        setItems([]);
    };

    const handleBlur = () => {
        toggle(false);
        setSearchQuery('');
    };

    const toggle = value => {
        setOpen(value);
    };

    const scroll = () => {
        console.log('scroll');
        if (counter < valuesList.length) {
            setCounter(prevState => prevState + PAGE_SIZE);
        }
    };

    const handleSearchChange = (e, {searchQuery}) => {
        setSearchQuery(searchQuery);
    };

    const handleFocus = () => {
        toggle(true);
    };

    /*let items =
        valuesList &&
        valuesList.map((x, index) => ({
            key: `${x.value}_${index}`,
            value: x.value,
            text: isTranslate ? t(x.name) : x.name,
        }));*/

    console.log('select', items);

    return (
        <Form.Field>
            {!noLabel ? (
                <label className={isDisabled ? 'label-disabled' : null}>{`${t(text || name)}${
                    isRequired ? ' *' : ''
                    }`}</label>
            ) : null}
            <div className="form-select">
                <Dropdown
                    placeholder={placeholder}
                    fluid
                    selection
                    loading={progress}
                    search
                    searchQuery={searchQuery}
                    text={textValue}
                    error={error}
                    disabled={isDisabled}
                    value={value}
                    closeOnChange={true}
                    closeOnBlur={true}
                    onBlur={handleBlur}
                    onClose={handleClose}
                    onOpen={handleOpen}
                    onSearchChange={handleSearchChange}
                    onFocus={handleFocus}
                    open={open}
                >
                    <div role="listbox" className={`menu transition`} ref={context}>
                        {items.map(item => (
                            <Dropdown.Item
                                key={item.value}
                                selected={item.value === value}
                                active={item.value === value}
                                value={item.value}
                                onClick={e => handleChange(e, {value: item.value})}
                            >
                                {item.name}
                            </Dropdown.Item>
                        ))}
                        <Visibility
                            continuous={false}
                            once={false}
                            context={context.current}
                            onTopVisible={scroll}
                            /*style={{
                                position: 'absolute',
                                bottom: 0,
                                left: 0,
                                right: 0,
                                zIndex: -1,
                            }}*/
                        />
                    </div>
                </Dropdown>
                {children && children}
            </div>
            {error && typeof error === 'string' && <span className="label-error">{error}</span>}
        </Form.Field>
    );
};

export default React.memo(Select);
