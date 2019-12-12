import React, {useState} from 'react';
import {Icon, Loader} from 'semantic-ui-react';
import * as Scroll from 'react-scroll/modules';
import {useTranslation} from 'react-i18next';

const Block = ({item, loading}) => {
    const {t} = useTranslation();

    let [open, setOpen] = useState(true);

    const toggleOpen = () => {
        setOpen(open => !open);
    };

    return (
        <Scroll.Element
            className="card-content-block"
            key={`block-item-${item.menuItem}`}
            name={item.menuItem}
        >
            <Loader active={loading} size="huge">
                Loading
            </Loader>
            <div className="card-content-block_header">
                <div>{t(item.menuItem)}</div>
                <div className="card-content-block_header_accordion" onClick={toggleOpen}>
                    <Icon name={open ? "angle down" : "angle up"}/>
                </div>
            </div>
            <div className={`card-content-block_${open ? 'open' : 'close'}`}>{item.render()}</div>
        </Scroll.Element>
    );
};

export default Block;
