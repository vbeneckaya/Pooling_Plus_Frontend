import React, {useRef} from 'react';
import {Button, Dropdown, Menu, Popup } from 'semantic-ui-react';
import {useDispatch, useSelector} from "react-redux";
import {
    exportFieldsSettingRequest,
    importFieldsSettingRequest,
    fieldsSettingSelector,
    exportProgressSelector,
    importProgressSelector,
    getFieldsSettingRequest
} from "../../../ducks/fieldsSetting";

const Header = ({gridsList, activeItem, changeActiveItem, rolesList, role, company, changeRole, t, changeCompany, companiesList = [], disabledCompany}) => {
    const rolesListOptions = [
        /*{ key: 'any_role', value: 'null', text: t('any_role') },*/
        ...rolesList.map(x => ({ key: x.name, value: x.value, text: x.name })),
    ];

    const companyListOptions = [
        {key: 'all', value: 'null', text: t('All')},
        ...companiesList.map(x => ({key: x.name, value: x.value, text: x.name}))
    ];

    const dispatch = useDispatch();
    const fileUploader = useRef(null);
    const fieldProperties =  useSelector(state => fieldsSettingSelector(state)) || [];

    const importLoader = useSelector(state => importProgressSelector(state));
    const exportLoader = useSelector(state => exportProgressSelector(state));

    const isImportBtn = true;
    const isExportBtn = true;

    const exportSettings = () => {
        dispatch(exportFieldsSettingRequest({
                forEntity: activeItem,
                fieldProperties: fieldProperties,
            }),
        );
    };

    const importSettings = () => {
        fileUploader && fileUploader.current.click();
    };


    const onFilePicked = e => {
        debugger;
        const file = e.target.files[0];

        const data = new FormData();
        data.append('FileName', file.name);
        data.append('FileContent', new Blob([file], { type: file.type }));
        data.append('FileContentType', file.type);

        dispatch(
            importFieldsSettingRequest({
                entity: activeItem,
                role: role,
                form: data,
                callbackSuccess: ()=>{dispatch(
                    getFieldsSettingRequest({
                        forEntity: activeItem,
                        roleId: /*role === 'null' ? undefined :*/ role,
                    }),
                );},
            }),
        );
    };
    
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
            <Menu.Menu position="right">
                {isImportBtn && (
                    <Popup
                        content={t('import')}
                        position="bottom right"
                        trigger={
                            <Button
                                icon="upload"
                                loading={importLoader}
                                onClick={importSettings}
                            />
                        }
                    />
                )}
                {isExportBtn && (
                    <Popup
                        content={
                            t('export')
                        }
                        position="bottom right"
                        trigger={
                            <Button
                                icon="download"
                                loading={exportLoader}
                                onClick={exportSettings}
                            />
                        }
                    />
                )}</Menu.Menu>
            <input
                type="file"
                ref={fileUploader}
                style={{display: 'none'}}
                onInput={onFilePicked}
            />
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
