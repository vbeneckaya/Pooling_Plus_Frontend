import React from 'react';
import {GRID_CARD_LINK, GRID_NEW_LINK} from "../../../../router/links";
import MiniOrdersGrid from "../../../../components/MiniGrid";
import {useSelector} from "react-redux";
import {settingsFormExtSelector} from "../../../../ducks/gridCard";

const Orders = (props) => {
    
    const {form , columns, isDeleteBtn, removeFromShipping, openOrderModal} = props;
    const {name = ''} = 'orders';

    return (
            <MiniOrdersGrid
                key={name}
                rows={form && form.orders}
                columns={columns}
                name={name}
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
