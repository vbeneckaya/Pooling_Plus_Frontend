import React from 'react';
import MiniOrdersGrid from "../../../../components/MiniGrid";
import {ORDERS_GRID} from "../../../../constants/grids";

const Orders = (props) => {
    
    const {form , columns, isDeleteBtn, removeFromShipping, openOrderModal, loadCard} = props;
    const {name = ''} = ORDERS_GRID;

    return (
            <MiniOrdersGrid
                key={name}
                rows={form && form.orders}
                columns={columns}
                name={ORDERS_GRID}
                loadCard={loadCard}
                // getActions={getActions}
                // modalCard={this.modalCard}
                removeFromShipping={removeFromShipping}
                isDeleteBtn={isDeleteBtn}
                openOrderModal={openOrderModal}
                //confirmation={confirmation}
                //closeConfirmation={this.closeConfirmation}
                //newLink={isCreateBtn ? GRID_NEW_LINK : null}
                //cardLink={GRID_CARD_LINK}
            />
    );
};

export default Orders;
