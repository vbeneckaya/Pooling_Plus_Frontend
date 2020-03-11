import React, { Component } from 'react';
import { connect } from 'react-redux';

import TableInfo from '../../components/TableInfo';
import { usersColumns } from '../../constants/usersColumns';
import {
    getUsersRequest,
    progressSelector,
    toggleUserActiveRequest,
    totalCountSelector,
    usersListSelector,
} from '../../ducks/users';
import { withTranslation } from 'react-i18next';
//import UserCard from './user_card_edit';
import { Button, Icon } from 'semantic-ui-react';
import {
    DICTIONARY_CARD_LINK,
    DICTIONARY_NEW_LINK,
    NEW_USER_LINK,
    USER_LINK,
} from '../../router/links';

// const newModal = (t, load, role) => (
//     <UserCard title={t('create_user_title')} id={null} loadList={load} role={role}>
//         <Button icon="add" />
//     </UserCard>
// );

export class UsersList extends Component {
    handleToggleIsActive = (event, { itemID, checked }, load) => {
        const { toggleActive } = this.props;
        toggleActive({
            id: itemID,
            active: checked,
            callbackSuccess: () => {
                load(false, true);
            },
        });
    };

    // getActions = (row, load, t , user) => {
    //     return [
    //         <UserCard id={row.id} title={t('edit_user', { name: row.userName })} loadList={load} role={user.role}>
    //             <Button size="mini" className="grid-action-btn">
    //                 <Icon name="edit" /> {t('edit_btn')}
    //             </Button>
    //         </UserCard>,
    //     ];
    // };
    
    // getCard = ({row, loadList, name}) => {
    //     const { t } = this.props;
    //
    //     return (
    //         <UserCard
    //             id={row.id}
    //             title={t('edit_user', {name: row.userName})}
    //             loadList={loadList}
    //         />
    //     );
    // };

    render() {
        const { list, loadList, totalCount, loading, t} = this.props;

        return (
            <TableInfo
                headerRow={usersColumns}
                name="users"
                loading={loading}
                className="wider ui container container-margin-top-bottom"
                list={list}
                toggleIsActive={this.handleToggleIsActive}
                totalCount={totalCount}
                loadList={loadList}
                newLink={NEW_USER_LINK}
                cardLink={USER_LINK}
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
        toggleActive: params => {
            dispatch(toggleUserActiveRequest(params));
        },
    };
};

export default withTranslation()(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(UsersList),
);
