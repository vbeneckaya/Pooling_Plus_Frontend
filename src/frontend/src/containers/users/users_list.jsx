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
    <UserCard title="Создание роли" id={null} loadList={load}>
        <Button size="small" color="blue" className="grid-action-btn">
            <Icon name="plus" /> Создать пользователя
        </Button>
    </UserCard>
);

export class UsersList extends Component {
    handleToggleIsActive = (event, { itemID, checked }) => {
        console.log('taggle', itemID, checked);
    };

    getActions = (row, load) => {
        return [
            <UserCard id={row.id} title={`Редактирование пользователя ${row.name}`} loadList={load}>
                <Button size="mini" className="grid-action-btn">
                    <Icon name="edit" /> Редактировать
                </Button>
            </UserCard>,
        ];
    };

    render() {
        const { list, loadList, totalCount, loading, t } = this.props;
        console.log('list', loadList);

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
