import React, { Component } from 'react';
import { connect } from 'react-redux';
import SuperGrid from '../../components/SuperGrid';
import {
    autoUpdateStart,
    autoUpdateStop,
    columnsGridSelector,
    getListRequest,
    listSelector,
    progressSelector,
    totalCountSelector,
} from '../../ducks/gridView';
import { withRouter } from 'react-router-dom';

class List extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const {
            columns = [],
            autoUpdate,
            stopUpdate,
            list,
            totalCount,
            progress,
            match = {},
        } = this.props;
        const { params = {} } = match;
        const { name = '' } = params;

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
    };
}

export default withRouter(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(List),
);
