import React, { Component } from 'react';
import { Button, Grid } from 'semantic-ui-react';
import Search from '../../../components/Search';

class Header extends Component {
    render() {
        const {
            createButton,
            searchValue,
            searchOnChange,
            counter,
            clearFilter,
            disabledClearFilter,
        } = this.props;
        return (
            <Grid className="grid-header-panel">
                <Grid.Row>
                    <Grid.Column width={10}>
                        {createButton}
                        <Search value={searchValue} onChange={searchOnChange} isAuto />
                        <span className="records-counter">
                            <b>{counter}</b> записей
                        </span>
                    </Grid.Column>
                    <Grid.Column width={6} className="grid-right-elements">
                        <Button color="orange" onClick={clearFilter} disabled={disabledClearFilter}>
                            Сбросить фильтры
                        </Button>
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        );
    }
}

export default Header;
