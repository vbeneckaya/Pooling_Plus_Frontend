import React from 'react';
import './style.scss';
import {Button, Icon} from 'semantic-ui-react';

const CardLayout = ({title, actionsFooter, actionsHeader, children, onClose}) => {
    return (
        <div>
            <div className="card-header-panel">
                <div className="card-header-panel_title">
                    <Button icon onClick={onClose}>
                        <Icon name="arrow left"/>
                    </Button>
                    {title}
                </div>
                {
                    actionsHeader && <div className="card-header-panel_actions">{actionsHeader()}</div>
                }
            </div>
            <div className="card-content">
                <div className="card-content-block">{children}</div>
            </div>
            <div className="card-actions-panel">{actionsFooter()}</div>
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
