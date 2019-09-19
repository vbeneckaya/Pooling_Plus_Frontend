import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';

import TableInfo from '../../components/TableInfo';
import {
    canCreateByFormSelector,
    columnsSelector,
    getListRequest,
    listSelector,
    progressSelector,
    totalCountSelector
} from '../../ducks/dictionaryView';
import {Button, Icon} from "semantic-ui-react";
import Card from "./card";


const newModal = (t, load, name) => (
    <Card title={t('create')} id={null} loadList={load} name={name}>
        <Button size="small" color="blue" className="grid-action-btn">
            <Icon name="plus" /> {t('create_btn')}
        </Button>
    </Card>
);

const List = ({ match = {}, columns, loadList, progress, totalCount, list, isCreateBtn }) => {

    const { params = {} } = match;
    const { name = '' } = params;

    return (
        <TableInfo
            headerRow={columns}
            name={name}
            className="wider container-margin-top-bottom"
            loadList={loadList}
            loading={progress}
            totalCount={totalCount}
            title={name}
            list={list}
            newModal={isCreateBtn ? newModal : null}
            modalCard={<Card title={'edit'} />}
        />
    );
};

const mapStateToProps = (state, ownProps) => {
    const { match = {} } = ownProps;
    const { params = {} } = match;
    const { name = '' } = params;

    console.log('name', name);

    return {
        columns: columnsSelector(state, name),
        progress: progressSelector(state),
        totalCount: totalCountSelector(state),
        list: listSelector(state),
        isCreateBtn: canCreateByFormSelector(state, name)
    };
};

const mapDispatchToProps = dispatch => {
    return {
        loadList: params => {
            dispatch(getListRequest(params));
        },
    };
};

export default withRouter(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(List),
);
