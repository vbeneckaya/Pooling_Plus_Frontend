import React from 'react';
import { Button, Icon, Table } from 'semantic-ui-react';
import { ORDERS_GRID } from '../../../constants/grids';
import { useTranslation } from 'react-i18next';

const NotFoundMessage = ({ isSetFilters, gridName, isCreateBtn, goToCard }) => {
    const { t } = useTranslation();

    const handleGoToCard = () => {
        goToCard(false, null, gridName);
    };

    return (
        <Table.Row>
            <Table.HeaderCell className="not-found-message">
                {isSetFilters ? (
                    t('No results were found for your request')
                ) : gridName === ORDERS_GRID ? (
                    <div>
                        <div>{t('You do not have invoices available')}</div>
                        {isCreateBtn && (
                            <Button onClick={handleGoToCard}>
                                <Icon name="add" />
                                {t('create_orders')}
                            </Button>
                        )}
                    </div>
                ) : (
                    t('You have no transportation, create invoices and combine them into transportation')
                )}
            </Table.HeaderCell>
        </Table.Row>
    );
};

export default NotFoundMessage;
