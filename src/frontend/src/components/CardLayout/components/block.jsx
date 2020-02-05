import React, {useState} from 'react';
import {Button, Icon, Loader, Popup} from 'semantic-ui-react';
import * as Scroll from 'react-scroll/modules';
import {useTranslation} from 'react-i18next';

const Block = ({item, loading}) => {
    const {t} = useTranslation();
    
    const {isCreateBtn , createAction} = item;

    let [open, setOpen] = useState(true);

    const toggleOpen = () => {
        setOpen(open => !open);
     //   console.log('item');
     //   console.log(item);
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
                <div className="card-content-block_header_accordion">
                    {isCreateBtn && (
                        <Popup
                            content={t('add_record')}
                            position="bottom right"
                            trigger={<Button icon="add" onClick={createAction}/>}
                        />)}
                    <Icon name={open ? 'angle down' : 'angle up'} onClick={toggleOpen}/>
                </div>
            </div>
            <div className={`card-content-block_${open ? 'open' : 'close'}`}>{item.render()}</div>
        </Scroll.Element>
    );
};

export default Block;
