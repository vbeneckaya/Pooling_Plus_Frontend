import React from 'react';
import { Button, Icon, Input, Popup } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';

import './style.scss';

const Facet = ({ name, text, value, onChange, sort, setSort }) => {
    const { t } = useTranslation();

    const input = (
        <Input
            fluid
            name={name}
            value={value || ''}
            autoComplete="off"
            label={{ basic: true, content: '' }}
            labelPosition="right"
            placeholder={t(name)}
            onChange={onChange}
        />
    );

    return (
        <div className="facet-input">
            <Input
                fluid
                name={name}
                value={value || ''}
                autoComplete="off"
                placeholder={t(name)}
                autoFocus
                onChange={onChange}
            />
            {/*<Popup trigger={input} content={t(name)} className="from-popup" on="focus" />
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
            </Button>*/}
        </div>
    );
};
export default Facet;
