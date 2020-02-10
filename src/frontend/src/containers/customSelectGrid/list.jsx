import React, { Component } from 'react';
import { connect } from 'react-redux';
import SelectGrid from '../../components/SelectGrid';
import {
    autoUpdateStart,
    autoUpdateStop,
    canCreateByFormSelector,
    listSelector,
    progressSelector,
    totalCountSelector,
} from '../../ducks/gridList';
import { withRouter } from 'react-router-dom';
import { withTranslation } from 'react-i18next';
import {
    actionsSelector,
    getAllIdsRequest,
} from '../../ducks/gridActions';
import {
    editRepresentationRequest,
    getRepresentationsRequest,
    // representationFromGridSelector,
    representationNameSelector,
} from '../../ducks/representations';
import {
    GRID_CARD_LINK,
    GRID_NEW_LINK,
} from '../../router/links';
import {ordersMiniColumns} from "../../constants/ordersMiniColumns";
import {ORDERS_GRID} from "../../constants/grids";

class List extends Component {
    constructor(props) {
        super(props);

        this.state = {
            confirmation: { open: false },
        };
    }

    // componentWillUnmount() {
    //     //this.props.stopUpdate({ isClear: true });
    // }

    // mapActions = (item, t, invokeAction, name) => ({
    //     ...item,
    //     name: `${t(item.name)}`,
    //     description: item.description,
    //     action: (rows, clearSelectedRows) => {
    //         this.showConfirmation(
    //             `${t('Are you sure to complete')} "${t(item.name)}" ${
    //                 rows.length > 1 ? `${t('for')} ` + rows.length : ''
    //             }?`,
    //             () => {
    //                 this.closeConfirmation();
    //                 invokeAction({
    //                     ids: rows,
    //                     name,
    //                     actionName: item.name,
    //                     callbackSuccess: () => {
    //                         clearSelectedRows();
    //                     },
    //                 });
    //             },
    //         );
    //     },
    // });

    // getGroupActions = () => {
    //     const { t, actions, invokeAction, match } = this.props;
    //     const { params = {} } = match;
    //     const { name = '' } = ORDERS_GRID; //params;
    //     const actionsFromGrid = actions.filter(item => item.allowedFromGrid);
    //
    //     let obj = {
    //         require: actionsFromGrid
    //             .filter((item, index) => index < 3)
    //             .map(item => this.mapActions(item, t, invokeAction, name)),
    //         other: actionsFromGrid
    //             .filter((item, index) => index >= 3)
    //             .map(item => this.mapActions(item, t, invokeAction, name)),
    //         order: actionsFromGrid
    //             .filter((item, index) => index >= 3)
    //             .filter(item => item.group === 'Order')
    //             .map(item => this.mapActions(item, t, invokeAction, name)),
    //         shipping: actionsFromGrid
    //             .filter((item, index) => index >= 3)
    //             .filter(item => item.group === 'Shipping')
    //             .map(item => this.mapActions(item, t, invokeAction, name)),
    //     };
    //
    //     return obj;
    // };

    showConfirmation = (content, onConfirm) => {
        this.setState({ confirmation: { open: true, onConfirm, content } });
    };

    closeConfirmation = () => {
        this.setState({ confirmation: { open: false } });
    };

    render() {
        const {
            columns = [],
            autoUpdate,
            stopUpdate,
            list,
            totalCount,
            progress,
            match = {},
            t,
           // isCreateBtn,
          //  getActions,
            getAllIds,
            selectCallback,
            editRepresentation,
            representationName,
            getRepresentations,
        } = this.props;
        const { params = {} } = match;
        const { name = '' } = params;
        const { confirmation } = this.state;
        
        return (
            <div className="container">
                <SelectGrid
                    key={ORDERS_GRID}
                    columns={columns}
                    rows={list}
                    name={ORDERS_GRID}
                    editRepresentation={editRepresentation}
                    representationName={representationName}
                    getRepresentations={getRepresentations}
                    autoUpdateStart={autoUpdate}
                    autoUpdateStop={stopUpdate}
                    totalCount={totalCount}
                    progress={progress}
                    storageSortItem={`${name}Sort`}
                    storageFilterItem={`${name}Filters`}
                    storageRepresentationItems={`${name}Representation`}
                 //   getActions={getActions}
                 //   groupActions={this.getGroupActions}
                    getAllIds={getAllIds}
                    selectCallback={selectCallback}
                 //   modalCard={this.modalCard}
                 //   isCreateBtn={isCreateBtn}
                  //  confirmation={confirmation}
                  //  closeConfirmation={this.closeConfirmation}
                  //  newLink={isCreateBtn ? GRID_NEW_LINK : null}
                    //cardLink={GRID_CARD_LINK}
                />
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => {
    return {
        autoUpdate: params => {
            dispatch(autoUpdateStart(params));
        },
        stopUpdate: params => {
            dispatch(autoUpdateStop(params));
        },
        getAllIds: params => {
            params.name = ORDERS_GRID;
            dispatch(getAllIdsRequest(params));
        },
        editRepresentation: params => {
            // params.name = ORDERS_GRID;
            dispatch(editRepresentationRequest(params));
        },
        getRepresentations: params => {
            // params.name = ORDERS_GRID;
            dispatch(getRepresentationsRequest(params));
        },
    };
};

const mapStateToProps = (state, ownProps) => {
  //  const { match = {} } = ownProps;
  //  const { params = {} } = match;
    const { name = '' } = ORDERS_GRID; //params;

    return {
        columns: ordersMiniColumns, //representationFromGridSelector(state, name),
        list: listSelector(state),
        totalCount: totalCountSelector(state),
        progress: progressSelector(state),
       // isCreateBtn: canCreateByFormSelector(state, name),
      //  actions: actionsSelector(state),
        representationName: representationNameSelector(state, name),
    };
};

export default withTranslation()(
    withRouter(
        connect(
            mapStateToProps,
            mapDispatchToProps,
        )(List),
    ),
);
