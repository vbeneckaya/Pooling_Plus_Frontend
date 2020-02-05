import React from 'react';
import {GRID_CARD_LINK, GRID_NEW_LINK} from "../../../../router/links";
import {ordersMiniColumns} from "../../../../constants/ordersMiniColumns";
import MiniOrdersGrid from "../../../../components/MiniGrid";

const Orders = (props) => {
    
    const {form , isCreateBtn, openOrderModal} = props;
    const {name = ''} = 'orders';
    const columns = ordersMiniColumns; 

    return (
            <MiniOrdersGrid
                key={name}
                rows={form && form.orders}
                columns={columns}
                name={name}
                // getActions={getActions}
                // modalCard={this.modalCard}
                isCreateBtn={isCreateBtn}
                openOrderModal={openOrderModal}
                //confirmation={confirmation}
                //closeConfirmation={this.closeConfirmation}
                //newLink={isCreateBtn ? GRID_NEW_LINK : null}
                //cardLink={GRID_CARD_LINK}
            />
    );
};

export default Orders;
