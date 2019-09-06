import React from 'react';
import { Button, Form, Icon, Input, Popup } from 'semantic-ui-react';
import DatePicker from 'react-datepicker';
import { dateToString, parseDate } from '../../utils/dateTimeFormater';

const Facet = ({ name, text, value, onChange, sort, setSort }) => {
    const getStringItem = i => {
        const parts = (value || '').split('-');
        return parts[i] || null;
    };

    const getDateItem = i => {
        let s = getStringItem(i);
        if (s) return parseDate(s);
        return null;
    };

    const callbackOnChange = (start, end) => {
        let value = start || end ? [start, end].join('-') : '';
        if (onChange !== undefined) onChange(new Event('change'), { name, value });
    };

    const toggleStart = value => {
        let start = dateToString(value);
        if (start == getStringItem(0)) start = null;
        callbackOnChange(start, getStringItem(1));
    };

    const toggleEnd = value => {
        let end = dateToString(value);
        if (end == getStringItem(1)) end = null;
        callbackOnChange(getStringItem(0), end);
    };

    const input = (
        <Input
            fluid
            name={name}
            autoComplete="off"
            value={value || ''}
            placeholder={text}
            label={{ basic: true, content: '' }}
            labelPosition="right"
            onChange={onChange}
        />
    );

    const content = (
        <Form className="filter-popup">
            <div>{text}</div>
            <Form.Group>
                <Form.Field width={8}>
                    <DatePicker
                        inline
                        locale="ru"
                        selected={getDateItem(0) || null}
                        dateFormat="dd.MM.yyyy"
                        allowSameDay
                        onChange={toggleStart}
                    />
                </Form.Field>
                <Form.Field width={8}>
                    <DatePicker
                        inline
                        locale="ru"
                        selected={getDateItem(1) || null}
                        dateFormat="dd.MM.yyyy"
                        allowSameDay
                        onChange={toggleEnd}
                    />
                </Form.Field>
            </Form.Group>
        </Form>
    );

    return (
        <div className="facet-input">
            <Popup
                trigger={input}
                content={content}
                on="click"
                hideOnScroll
                className="from-popup"
                position="bottom center"
            />
            <Button
                className={`sort-button sort-button-up ${
                    sort === 'asc' ? 'sort-button-active' : ''
                }`}
                name={name}
                value="asc"
                onClick={setSort}
            >
                <Icon name="caret up" />
            </Button>
            <Button
                className={`sort-button sort-button-down ${
                    sort === 'desc' ? 'sort-button-active' : ''
                }`}
                name={name}
                value="desc"
                onClick={setSort}
            >
                <Icon name="caret down" />
            </Button>
        </div>
    );
};

export default Facet;
