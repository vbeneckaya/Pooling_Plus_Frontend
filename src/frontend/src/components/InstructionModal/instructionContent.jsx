import React from "react";

const InstructionContent = (pathName) => {
    const content = new Object();

    content["/grid/orders"] = {
        title: "Заказы",
        content:
            <label>instruction for /grid/orders</label>
    };

    content["/grid/shippings"] = {
        title: "Перевозки",
        content:
            <label>instruction  for /grid/shippings</label>
    };

    const res = content[pathName]
    
    return res;
}

export default InstructionContent;