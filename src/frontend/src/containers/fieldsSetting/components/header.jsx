import React from 'react';
import { Dropdown, Menu } from 'semantic-ui-react';
import _ from 'lodash';

const Header = ({gridsList, activeItem, changeActiveItem, rolesList, role, company, changeRole, t, changeCompany, companiesList = [], disabledCompany}) => {
    const rolesListOptions = [
        /*{ key: 'any_role', value: 'null', text: t('any_role') },*/
        ...rolesList.map(x => ({ key: x.name, value: x.value, text: x.name })),
    ];

    const companyListOptions = [
        {key: 'all', value: 'null', text: t('All')},
        ...companiesList.map(x => ({key: x.name, value: x.value, text: x.name}))
    ];

    return (
        <Menu className="field-settings-menu">
            {gridsList && gridsList.length
                ? gridsList.map(item => (
                      <Menu.Item
                          key={item}
                          active={activeItem === item}
                          name={item}
                          onClick={changeActiveItem}
                      >
                          {t(item)}
                      </Menu.Item>
                  ))
                : null}
            <Menu.Item className={"field-settings-menu_filter"}>
                {t('companyId')}{'  '}
                <Dropdown inline value={company} options={companyListOptions} onChange={changeCompany}
                          disabled={disabledCompany}/>
            </Menu.Item>
            <Menu.Item className={"field-settings-menu_filter"}>
                {t('role')}{'  '}
                <Dropdown value={role} inline options={rolesListOptions} onChange={changeRole}/>
            </Menu.Item>
            {/*<Menu.Item>
                    <Dropdown
                        value={company}
                        selection
                        options={companyListOptions}
                        onChange={(e, { value }) => {
                            setCompany(value);
                        }}
                    />
                </Menu.Item>*/}
        </Menu>
    );
};

export default React.memo(Header);
