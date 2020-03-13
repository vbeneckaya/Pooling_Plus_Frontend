import React from 'react';
import {withRouter} from 'react-router-dom';

import './style.scss';
import connect from "react-redux/es/connect/connect";
import {useDispatch} from "react-redux";
import {loginByTokenRequest} from "../../ducks/login";

const SignWithoutLogin = (props) => {
    const dispatch = useDispatch();
    const {userId, token, history} = props;
    dispatch(loginByTokenRequest({userId,token}));
    history.replace("/");
    return (null);
};


const mapStateToProps = (state, ownProps) => {
    const {match = {}} = ownProps;
    const {params = {}} = match;
    const {token = ''} = params;
    const {userId = ''} = params;

    return {
        token: token,
        userId: userId,
    };
};

export default withRouter(
    connect(
        mapStateToProps,
        null,
    )(SignWithoutLogin),
);
