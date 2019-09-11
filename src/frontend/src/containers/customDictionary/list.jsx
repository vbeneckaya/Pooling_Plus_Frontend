import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';

import TableInfo from '../../components/TableInfo';
import { columnsSelector, getListRequest } from '../../ducks/dictionaryView';

const List = ({ columns, loadList }) => {
    return (
        <TableInfo
            headerRow={columns}
            className="wider container-margin-top-bottom"
            loadList={loadList}
        />
    );
};

const mapStateToProps = (state, ownProps) => {
    const { match = {} } = ownProps;
    const { params = {} } = match;
    const { name = '' } = params;

    return {
        columns: columnsSelector(state, name),
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
