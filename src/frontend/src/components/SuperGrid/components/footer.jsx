import React, { useState } from 'react';

import { ORDERS_GRID } from '../../../constants/grids';
import { Button, Grid, Icon, Popup } from 'semantic-ui-react';
import Mass_changes from './mass_changes';

import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { infoSelector } from '../../../ducks/gridActions';

const InfoView = ({ info, t, handleClose }) => {
    return (
        <div className="footer-info">
            <div className="footer-info-close" onClick={handleClose}>
                <Icon name="sort down" />
            </div>
            <div>
                {t('orders_selected')}
                <span className="footer-info-value">{info.count}</span>
            </div>
            <div>
                {t('number_of_boxes')}
                <span className="footer-info-value">{info.boxesCount}</span>
            </div>
            <div>
                {t('number_of_pallets')}
                <span className="footer-info-value">{info.palletsCount}</span>
            </div>
            <div>
                {t('target_weight')}
                <span className="footer-info-value">{info.weightKg}</span>
            </div>
        </div>
    );
};

const Footer = ({ groupActions, load, clearSelectedRows, gridName }) => {
    const { t } = useTranslation();
    let [isOpen, setIsOpen] = useState(false);

    const info = useSelector(state => infoSelector(state));

    const handleOpen = () => {
        setIsOpen(true);
    };

    const handleClose = () => {
        setIsOpen(false);
    };

    return (
        <Grid className="grid-footer-panel" columns="2">
            <Grid.Row>
                <Grid.Column width={10}>
                    {gridName === ORDERS_GRID ? (
                        <Popup
                            trigger={
                                <div
                                    className="footer-info-label"
                                    onClick={isOpen ? handleClose : handleOpen}
                                >
                                    <Icon name={isOpen ? 'sort up' : 'sort down'} />
                                    Данные по заказам
                                </div>
                            }
                            content={<InfoView info={info} t={t} handleClose={handleClose} />}
                            on="click"
                            open={isOpen}
                            onClose={handleClose}
                            onOpen={handleOpen}
                            hideOnScroll
                            className="from-popup"
                        />
                    ) : null}
                    <div className="footer_actions">
                        {groupActions
                            ? groupActions().map(action => (
                                      <Button
                                          key={action.name}
                                          color={action.color}
                                          content={action.name}
                                          loading={action.loading}
                                          disabled={action.loading}
                                          icon={action.icon}
                                          size="mini"
                                          compact
                                          onClick={() =>
                                              action.action(action.ids, clearSelectedRows)
                                          }
                                      />
                              ))
                            : null}
                    </div>
                </Grid.Column>
                <Grid.Column floated="right">
                    <Mass_changes gridName={gridName} load={() => load(false, true)} />
                </Grid.Column>
            </Grid.Row>
        </Grid>
    );
};

export default Footer;
