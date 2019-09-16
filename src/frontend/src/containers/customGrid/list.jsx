import React, { Component } from 'react';
import { connect } from 'react-redux';
import SuperGrid from '../../components/SuperGrid';
import {
    autoUpdateStart,
    autoUpdateStop, canCreateByFormSelector,
    columnsGridSelector,
    getListRequest,
    listSelector,
    progressSelector,
    totalCountSelector,
} from '../../ducks/gridView';
import { withRouter } from 'react-router-dom';
import Card from "./card";
import {Button} from "semantic-ui-react";
import { withTranslation } from 'react-i18next';
import {actionsSelector, getActionsRequest} from "../../ducks/gridActions";

const CreateButton = ({t, ...res}) => (
    <Card title={t("create_form_title")} {...res} >
        <Button color="blue" className="create-button">
            {t("create_btn")}
        </Button>
    </Card>
);

class List extends Component {
    constructor(props) {
        super(props);
    }

    getGroupActions = () => {

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
            getActions
        } = this.props;
        const { params = {} } = match;
        const { name = '' } = params;

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
                    createButton={isCreateBtn ? <CreateButton t={t} /> : null }
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
            dispatch(getActionsRequest(params))
        }
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
        actions: actionsSelector(state)
    };
}

export default withTranslation()(withRouter(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(List)),
);
