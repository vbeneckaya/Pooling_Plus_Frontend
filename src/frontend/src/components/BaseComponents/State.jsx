import React from 'react';
import { Dropdown, Form, Icon } from 'semantic-ui-react';
import { useSelector } from 'react-redux';
import { stateColorsSelector } from '../../ducks/gridList';
import { useTranslation } from 'react-i18next';

const State = ({ value, name, isDisabled, onChange, className }) => {
    console.log('value', value);
    const stateColors = useSelector(state => stateColorsSelector(state));
    const state = stateColors.find(x => x.name === value);
    const color = state ? state.color : 'grey';
    const { t } = useTranslation();

    const items = (stateColors || []).map(x => {
        return { key: x.name, value: x.name, text: t(x.name), label:{ color: x.color, empty: true, circular: true } };
    });

    if (!isDisabled)
        return (
            <Form.Field>
                <label>{t(name)}</label>
                <Dropdown
                    className={className}
                    selection
                    search
                    disabled={isDisabled || false}
                    name={name}
                    value={value}
                    fluid
                    options={items}
                    onChange={onChange}
                >
                </Dropdown>
            </Form.Field>
        );
    else
        return (
            <Form.Field>
                <label>{t(name)}</label>
                <div className="semantic-ui-div state-edit-field">
                    <Icon color={color} name="circle" />
                    {t(value)}
                </div>
            </Form.Field>
        );
};
export default State;
