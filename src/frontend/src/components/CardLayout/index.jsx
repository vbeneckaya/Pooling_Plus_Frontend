import React, {useState, useEffect, useRef} from 'react';
import {useTranslation} from 'react-i18next';
import './style.scss';
import {Button, Dimmer, Icon, Loader, Menu} from 'semantic-ui-react';
import * as Scroll from 'react-scroll';
import Block from './components/block';

const CardLayout = ({
                        title,
                        actionsFooter,
                        actionsHeader,
                        children,
                        onClose,
                        content,
                        loading,
                    }) => {
    const {t} = useTranslation();

   // const contentRef = useRef(null);

    let [activeItem, setActiveItem] = useState();
    let [activeItemContent, setActiveItemContent] = useState();

    const handleItemClick = (e, {item}) => {
        setActiveItem(item);
        // Scroll &&
        // Scroll.scroller &&
        // Scroll.scroller.scrollTo &&
        // Scroll.scroller.scrollTo(item, {
        //     duration: 1500,
        //     delay: 100,
        //     offset: -120,
        // });
    };

    useEffect(
        () => {
            content && content() && content().length && setActiveItemContent(content()[0]);
            content && content() && content().length && setActiveItem(content()[0].menuItem);
        },
        [content],
    );
    
    useEffect(()=>{
        if(activeItem && content && content() && content().length ){
            
            setActiveItemContent(content().find(_=>_.menuItem == activeItem))
        }
        if(!activeItem && content && !content())
            setActiveItemContent(content)
        
    },[activeItem])

    return (
        <div>
            <div className="card-header-panel">
                <div className="card-header-panel_title">
                    <Button icon onClick={onClose}>
                        <Icon name="arrow left"/>
                    </Button>
                    {title}
                </div>
                {actionsHeader && (
                    <div className="card-header-panel_actions">{actionsHeader()}</div>
                )}
            </div>
            <div className={!!content && !!content() ? "grid-card-content" :"card-content"}>
                {content ? (
                    <div>
                        <Menu pointing className="grid-card-content_menu">
                            {content().map(item => (
                                <Menu.Item
                                    key={`menu-item-${item.menuItem}`}
                                    name={t(item.menuItem)}
                                    item={item.menuItem}
                                    active={activeItem === item.menuItem}
                                    to={item.menuItem}
                                    onClick={handleItemClick}
                                />
                            ))}
                        </Menu>
                        <div className={!!content &&!!content() ? "grid-card-content_block" : "card-content-block_menu"}>
                            {activeItemContent && <Block 
                                    item={activeItemContent} 
                                    loading={loading}
                                />}
                        </div>
                    </div>
                ) : (
                    <div className="card-content-block">
                        <Loader active={loading} size="huge">
                            Loading
                        </Loader>
                        {children}
                    </div>
                )}
            </div>
            {actionsFooter && <div className="card-actions-panel">
                <div
                    // style={{
                    //     width:
                    //         contentRef && contentRef.current && contentRef.current.offsetWidth - 64,
                    // }}
                >
                    {actionsFooter()}
                </div>
            </div>}
            <style>
                {
                    '\
                body{\
                  overflow:auto;\
                }\
            '
                }
            </style>
        </div>
    );
};

export default CardLayout;
