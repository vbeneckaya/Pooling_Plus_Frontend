import React, { Component } from 'react';
import {Button, Form, Grid, Popup} from 'semantic-ui-react';
import Search from '../../../components/Search';
import { useTranslation } from 'react-i18next';
import Select from "../../BaseComponents/Select";

const Header = ({
    createButton,
    searchValue,
    searchOnChange,
    counter,
    clearFilter,
    disabledClearFilter,
    isImportBtn,
}) => {
    const { t } = useTranslation();

    const importExcel = () => {

    };

    const settings = () => {

    };

    return (
        <Grid className="grid-header-panel">
            <Grid.Row>
                <Grid.Column width={10}>
                    {createButton}
                    <Search value={searchValue} onChange={searchOnChange} isAuto />
                    <span className="records-counter">{t('totalCount', { count: counter })}</span>
                </Grid.Column>
                <Grid.Column width={6} className="grid-right-elements">
                    <div className="representation">
                        <label>{t('representation')}</label>
                        <Select/>
                        <Button
                            icon="cogs"
                            onClick={settings}
                        />
                    </div>
                    <Popup
                        content={t('reset_filters')}
                        position='bottom right'
                        trigger={
                            <Button
                                icon="times"
                                onClick={clearFilter}
                                disabled={disabledClearFilter}
                            />
                        }
                    />
                    {
                        isImportBtn &&  <Popup
                            content={t('importFromExcel')}
                            position='bottom right'
                            trigger={
                                <Button
                                    icon="file excel"
                                    onClick={importExcel}
                                />
                            }
                        />
                    }
                </Grid.Column>
            </Grid.Row>
        </Grid>
    );
};

export default Header;
