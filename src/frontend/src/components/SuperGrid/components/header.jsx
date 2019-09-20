import React, { Component } from 'react';
import { Button, Grid } from 'semantic-ui-react';
import Search from '../../../components/Search';
import { useTranslation } from 'react-i18next';

const Header = ({
    createButton,
    searchValue,
    searchOnChange,
    counter,
    clearFilter,
    disabledClearFilter,
}) => {

    const { t } = useTranslation();

    return (
        <Grid className="grid-header-panel">
            <Grid.Row>
                <Grid.Column width={10}>
                    {createButton}
                    <Search value={searchValue} onChange={searchOnChange} isAuto />
                    <span className="records-counter">
                        {t('totalCount', {count: counter})}
                    </span>
                </Grid.Column>
                <Grid.Column width={6} className="grid-right-elements">
                    <Button color="orange" onClick={clearFilter} disabled={disabledClearFilter}>
                        {t('reset_filters')}
                    </Button>
                </Grid.Column>
            </Grid.Row>
        </Grid>
    );
};

export default Header;
