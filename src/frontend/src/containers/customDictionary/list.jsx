import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';

import TableInfo from '../../components/TableInfo';
import {
    columnsSelector,
    getListRequest,
    listSelector,
    progressSelector,
    totalCountSelector
} from '../../ducks/dictionaryView';

const List = ({ match = {}, columns, loadList, progress, totalCount, list }) => {

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
            list={list}
        />
    );
};

const mapStateToProps = (state, ownProps) => {
    const { match = {} } = ownProps;
    const { params = {} } = match;
    const { name = '' } = params;

    return {
        columns: columnsSelector(state, name),
        progress: progressSelector(state),
        totalCount: totalCountSelector(state),
        list: listSelector(state)
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
