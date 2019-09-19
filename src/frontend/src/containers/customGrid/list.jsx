import React, { Component } from 'react';
import { connect } from 'react-redux';
import SuperGrid from '../../components/SuperGrid';
import {
    autoUpdateStart,
    autoUpdateStop,
    canCreateByFormSelector,
    columnsGridSelector,
    getListRequest,
    listSelector,
    progressSelector,
    totalCountSelector,
} from '../../ducks/gridList';
import { withRouter } from 'react-router-dom';
import Card from './card';
import { Button } from 'semantic-ui-react';
import { withTranslation } from 'react-i18next';
import { actionsSelector, getActionsRequest, invokeActionRequest } from '../../ducks/gridActions';

const CreateButton = ({ t, ...res }) => (
    <Card title={t('create_form_title')} {...res}>
        <Button color="blue" className="create-button">
            {t('create_btn')}
        </Button>
    </Card>
);

class List extends Component {
    constructor(props) {
        super(props);

        this.state = {
            confirmation: { open: false },
        };
    }

    getGroupActions = () => {
        const { t, actions, invokeAction, match } = this.props;
        const { params = {} } = match;
        const { name = '' } = params;

        return actions.map(item => ({
            ...item,
            name: `${t(item.name)} ${item.ids.length > 1 ? `(${item.ids.length})` : ''}`,
            action: (rows, clearSelectedRows) => {
                this.showConfirmation(`${t('Are you sure to complete')} "${t(item.name)}" ${rows.length> 1 ? `${t('for')} ` + rows.length : ''}?`, () => {
                    this.closeConfirmation();
                    invokeAction({
                        ids: rows,
                        name,
                        actionName: item.name,
                        callbackSuccess: () => {
                            clearSelectedRows();
                        },
                    });
                });
            },
        }));
    };

    showConfirmation = (content, onConfirm) => {
        this.setState({ confirmation: { open: true, onConfirm, content } });
    };

    closeConfirmation = () => {
        this.setState({ confirmation: { open: false } });
    };

    render() {
        const {
            columns = [],
            autoUpdate,
            stopUpdate,
            list,
            totalCount,
            progress,
            match = {},
            t,
            isCreateBtn,
            getActions,
        } = this.props;
        const { params = {} } = match;
        const { name = '' } = params;
        const { confirmation } = this.state;

        console.log('isCreateBtn', isCreateBtn);

        return (
            <div className="container">
                <SuperGrid
                    columns={columns}
                    rows={list}
                    name={name}
                    autoUpdateStart={autoUpdate}
                    autoUpdateStop={stopUpdate}
                    totalCount={totalCount}
                    progress={progress}
                    storageSortItem={`${name}Sort`}
                    storageFilterItem={`${name}Filters`}
                    getActions={getActions}
                    groupActions={this.getGroupActions}
                    createButton={isCreateBtn ? <CreateButton t={t} /> : null}
                    confirmation={confirmation}
                    closeConfirmation={this.closeConfirmation}
                />
            </div>
        );
    }
}

function mapDispatchToProps(dispatch) {
    return {
        autoUpdate: params => {
            dispatch(autoUpdateStart(params));
        },
        stopUpdate: params => {
            dispatch(autoUpdateStop(params));
        },
        getActions: params => {
            dispatch(getActionsRequest(params));
        },
        invokeAction: params => {
            dispatch(invokeActionRequest(params));
        },
    };
}

function mapStateToProps(state, ownProps) {
    const { match = {} } = ownProps;
    const { params = {} } = match;
    const { name = '' } = params;

    return {
        columns: columnsGridSelector(state, name),
        list: listSelector(state),
        totalCount: totalCountSelector(state),
        progress: progressSelector(state),
        isCreateBtn: canCreateByFormSelector(state, name),
        actions: actionsSelector(state),
    };
}

export default withTranslation()(
    withRouter(
        connect(
            mapStateToProps,
            mapDispatchToProps,
        )(List),
    ),
);
