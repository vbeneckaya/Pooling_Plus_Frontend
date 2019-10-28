import React from 'react';
import { useTranslation } from 'react-i18next';
import { Popup, Input, Button, Icon, Form, Dropdown } from 'semantic-ui-react';

const Bool = ({ value, sort, name, setSort, text, onChange }) => {
    let items = [
        {
            text: 'Не выбрано',
            value: undefined,
        },
        {
            text: 'Да',
            value: true,
        },
        {
            text: 'Нет',
            value: false,
        },
    ];

    const { t } = useTranslation();

    const handleChange = value => {
        if (onChange !== undefined) onChange(null, { name: name, value: value });
    };

    let content = (
        <Form.Field>
            <Form>
                <label className="label-in-popup">{t(name)}</label>
                <div className="boolean-facet-values">
                    {items &&
                        items.map(x => {
                            return (
                                <div
                                    key={x.text}
                                    className={x.value === value ? 'active-value' : ''}
                                    onClick={() => handleChange(x.value)}
                                >
                                    <span>{x.text}</span>
                                </div>
                            );
                        })}
                </div>
            </Form>
        </Form.Field>
    );

    return (
        <div className="facet-input">
            <Popup
                trigger={
                    <Input
                        fluid
                        label={{ basic: true, content: '' }}
                        labelPosition="right"
                        onKeyPress={e => {
                            e.preventDefault();
                        }}
                        placeholder={
                            value !== undefined
                                ? items.find(item => item.value === value) &&
                                  items.find(item => item.value === value).text
                                : t(name)
                        }
                    />
                }
                content={content}
                on="click"
                hoverable
                className="from-popup"
                position="bottom left"
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

export default Bool;
