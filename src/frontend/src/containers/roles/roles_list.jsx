import React, { Component } from 'react';
import { connect } from 'react-redux';

import TableInfo from '../../components/TableInfo';

import { rolesColumns } from '../../constants/rolesColumns';
import {
    getRolesRequest,
    progressSelector,
    rolesListSelector,
    totalCountSelector,
} from '../../ducks/roles';
import { Button, Icon } from 'semantic-ui-react';
import RoleCard from './role_card';

const newModal = load => (
    <RoleCard title="Создание роли" id={null} loadList={load}>
        <Button size="small" color="blue" className="grid-action-btn">
            <Icon name="plus" /> Создать роль
        </Button>
    </RoleCard>
);

export class RolesList extends Component {
    handleToggleIsActive = (event, { itemID, checked }) => {
        console.log('toggle', itemID, checked);
    };

    getActions = (row, load) => {
        return [
            <RoleCard id={row.id} title={`Редактировать роль ${row.name}`} loadList={load}>
                <Button size="mini" className="grid-action-btn">
                    <Icon name="edit" /> Редактировать
                </Button>
            </RoleCard>,
        ];
    };

    render() {
        const { list, loadList, totalCount, loading } = this.props;
        console.log('list', list);

        return (
            <TableInfo
                headerRow={rolesColumns}
                title="Роли"
                loading={loading}
                className="wider container-margin-top-bottom"
                list={list}
                isShowActions
                actions={this.getActions}
                newModal={newModal}
                toggleIsActive={this.handleToggleIsActive}
                totalCount={totalCount}
                loadList={loadList}
            />
        );
    }
}

const mapStateToProps = state => {
    return {
        list: rolesListSelector(state),
        totalCount: totalCountSelector(state),
        loading: progressSelector(state),
    };
};

const mapDispatchToProps = dispatch => {
    return {
        loadList: params => {
            dispatch(getRolesRequest(params));
        },
    };
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(RolesList);
