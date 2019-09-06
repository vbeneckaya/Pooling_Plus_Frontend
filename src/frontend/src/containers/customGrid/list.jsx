import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';

class List extends Component {
    constructor(props) {
        super(props);
    }
    render() {
        return <div className="List" />;
    }
}
List.propTypes = {};

function mapDispatchToProps(dispatch) {
    return {};
}

function mapStateToProps(state, ownProps) {
    return {};
}

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(List);
