import React, { Component } from 'react';
import { connect } from 'react-redux';

import TableInfo from '../../components/TableInfo';
import { usersColumns } from '../../constants/usersColumns';
import {
    getUsersRequest,
    progressSelector,
    totalCountSelector,
    usersListSelector,
} from '../../ducks/users';
import { withTranslation } from 'react-i18next';
import UserCard from './user_card';
import { Button, Icon } from 'semantic-ui-react';


const newModal = (t, load) => (
    <UserCard title={t('create_user_title')} id={null} loadList={load}>
        <Button size="small" color="blue" className="grid-action-btn">
            <Icon name="plus" /> {t('create_user')}
        </Button>
    </UserCard>
);

export class UsersList extends Component {
    handleToggleIsActive = (event, { itemID, checked }) => {
    };

    getActions = (row, load, t) => {
        return [
            <UserCard id={row.id} title={t('edit_user', {name: row.userName})} loadList={load}>
                <Button size="mini" className="grid-action-btn">
                    <Icon name="edit" /> Редактировать
                </Button>
            </UserCard>,
        ];
    };

    render() {
        const { list, loadList, totalCount, loading, t } = this.props;

        return (
            <TableInfo
                headerRow={usersColumns}
                title="Пользователи"
                loading={loading}
                className="wider container-margin-top-bottom"
                list={list}
                isShowActions
                actions={this.getActions}
                toggleIsActive={this.handleToggleIsActive}
                totalCount={totalCount}
                newModal={newModal}
                loadList={loadList}
            />
        );
    }
}

const mapStateToProps = state => {
    return {
        list: usersListSelector(state),
        totalCount: totalCountSelector(state),
        loading: progressSelector(state),
    };
};

const mapDispatchToProps = dispatch => {
    return {
        loadList: params => {
            dispatch(getUsersRequest(params));
        },
    };
};

export default withTranslation()(connect(
    mapStateToProps,
    mapDispatchToProps,
)(UsersList));
