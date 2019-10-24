import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withTranslation } from 'react-i18next';

import TableInfo from '../../components/TableInfo';

import { rolesColumns } from '../../constants/rolesColumns';
import {
    getRolesRequest,
    progressSelector,
    rolesListSelector,
    toggleRoleActiveRequest,
    totalCountSelector,
} from '../../ducks/roles';
import { Button, Icon } from 'semantic-ui-react';
import RoleCard from './role_card';

const newModal = (t, load) => (
    <RoleCard title={t('create_role_title')} id={null} loadList={load}>
        <Button size="small" color="blue" className="grid-action-btn">
            <Icon name="plus" /> {t('create_role')}
        </Button>
    </RoleCard>
);

export class RolesList extends Component {
    handleToggleIsActive = (event, { itemID, checked }, load) => {
        const { toggleActive } = this.props;

        toggleActive({
            id: itemID,
            active: checked,
            callbackSuccess: () => {
                load();
            },
        });
    };

    getActions = (row, load, t) => {
        return [
            <RoleCard id={row.id} title={t('edit_role', { name: row.name })} loadList={load}>
                <Button size="mini" className="grid-action-btn">
                    <Icon name="edit" /> {t('edit_btn')}
                </Button>
            </RoleCard>,
        ];
    };

    render() {
        const { list, loadList, totalCount, loading, t } = this.props;

        return (
            <TableInfo
                headerRow={rolesColumns}
                title={t('roles')}
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
        toggleActive: params => {
            dispatch(toggleRoleActiveRequest(params));
        },
    };
};

export default withTranslation()(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(RolesList),
);
