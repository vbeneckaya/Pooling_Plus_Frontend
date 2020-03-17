import React from "react";
// import {useTranslation} from "react-i18next";
// import {ORDERS_GRID, SHIPPINGS_GRID} from "../../constants/grids";


const InstructionContent = (pathName) => {
    // const {t} = useTranslation();
    const content = new Object();

    content['/grid/orders'] = {
        title: "Заказы",
        content:
            <label>instruction for /grid/orders</label>
    };

    content['/grid/shippings'] = {
        title: "Перевозки",
        content:
            <label>instruction for /grid/shippings</label>
    };

    const res = content[pathName]
    
    return res;
}

export default InstructionContent;