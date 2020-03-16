import React from "react";

export const inctructionContent = (pathName) => {
    const content = new Object();

    content['/grid/orders'] = <label>instruction for /grid/orders</label>;

    content['/grid/shippings'] = <label>instruction for /grid/shippings</label>;
    
    const res = content[pathName]
    
    return res;
}