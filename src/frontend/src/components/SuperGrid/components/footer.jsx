import React, { useState } from 'react';

import { ORDERS_GRID } from '../../../constants/grids';
import {Button, Dropdown, Grid, Icon, Popup} from 'semantic-ui-react';
import MassChanges from './mass_changes';

import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { infoSelector } from '../../../ducks/gridActions';

const InfoView = ({info, t, handleClose, gridName, selectedRowsLen}) => {
    return (
        <div className="footer-info">
            {/*<div className="footer-info-close" onClick={handleClose}>
                <Icon name="sort down" />
            </div>*/}
            {gridName === ORDERS_GRID ? (
                <>
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
                </>
            ) : (
                <div>
                    {t('shippings_selected')}
                    <span className="footer-info-value">{selectedRowsLen}</span>
                </div>
            )}
        </div>
    );
};

const Footer = ({groupActions, load, clearSelectedRows, gridName, selectedRows}) => {
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
                    <InfoView
                        info={info}
                        t={t}
                        handleClose={handleClose}
                        gridName={gridName}
                        selectedRowsLen={selectedRows.size}
                    />
                    {/*{gridName === ORDERS_GRID ? (

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
                    ) : null}*/}
                    <div className="footer_actions">
                        {/* {groupActions
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
                            : null}*/}
                        {groupActions
                            ? groupActions().require.map(action => (
                                <Button
                                    key={action.name}
                                    color={action.color}
                                    content={action.name}
                                    loading={action.loading}
                                    disabled={action.loading}
                                    icon={action.icon}
                                    size="mini"
                                    compact
                                    onClick={() => action.action(action.ids, clearSelectedRows)}
                                />
                            ))
                            : null}
                        {
                            groupActions &&
                            groupActions().other.length
                                ? <Dropdown
                                    icon="ellipsis horizontal"
                                    floating
                                    button
                                    upward
                                    className="icon mini ellipsis-actions-btn"
                            >
                                <Dropdown.Menu>
                                    <Dropdown.Menu scrolling>
                                        {groupActions().other.map(action => (
                                            <Dropdown.Item
                                                key={action.name}
                                                text={action.name}
                                                label={{color: action.color, empty: true, circular: true}}
                                                onClick={() => action.action(action.ids, clearSelectedRows)}
                                            />
                                        ))}
                                    </Dropdown.Menu>
                                </Dropdown.Menu>
                            </Dropdown>
                                : null
                        }
                    </div>
                </Grid.Column>
                <Grid.Column floated="right">
                    <MassChanges gridName={gridName} load={() => load(false, true)}/>
                </Grid.Column>
            </Grid.Row>
        </Grid>
    );
};

export default Footer;
