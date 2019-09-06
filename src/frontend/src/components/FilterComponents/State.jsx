import React from 'react';
import { Button, Popup, Checkbox, Icon, Form } from 'semantic-ui-react';

const Facet = ({ value, onChange, stateColors, sort, setSort, name }) => {
    let values = value ? value.split('|') : [];

    const toggle = (e, { value }) => {
        if (values.some(x => x === value)) {
            values.splice(values.indexOf(value), 1);
        } else {
            values.push(value);
        }
        if (onChange !== undefined) onChange(e, { name, value: values.join('|') });
    };

    let content = (
        <Form>
            {stateColors.map(x => {
                let label = (
                    <label>
                        <Icon color={x.color} inverted={x.inverted} name="circle" />
                        {x.text}
                    </label>
                );
                return (
                    <Form.Field key={x.value}>
                        <Checkbox
                            value={x.value}
                            checked={values.includes(x.value)}
                            onChange={toggle}
                            label={label}
                        />
                    </Form.Field>
                );
            })}
        </Form>
    );

    return (
        <div className="facet-sortable facet-input">
            <Popup
                trigger={
                    <Button size="small" style={{ lineHeight: '1.1rem' }} fluid>
                        {values.length > 0 ? values.length + ' ' + 'выбрано' : 'Все'}
                    </Button>
                }
                content={content}
                on="click"
                className="from-popup"
                hideOnScroll
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
export default Facet;
