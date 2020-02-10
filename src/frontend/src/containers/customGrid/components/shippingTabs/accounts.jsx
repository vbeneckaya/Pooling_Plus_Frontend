import React from 'react';
import { Form, Grid, Segment } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';
import { BIG_TEXT_TYPE, TEXT_TYPE, CHECKBOX_TYPE, NUMBER_TYPE } from '../../../../constants/columnTypes';
import FormField from '../../../../components/BaseComponents';

const Accounts = ({ form = {}, onChange, onBlur, settings }) => {
    const { t } = useTranslation();

    return (
        <Form className="tabs-card">
            <Grid>
                <Grid.Row columns={2} stretched>
                    <Grid.Column>
                        <FormField
                            name="deliveryCostWithoutVAT"
                            value={form['deliveryCostWithoutVAT']}
                            type={NUMBER_TYPE}
                            settings={settings['deliveryCostWithoutVAT']}
                            onChange={onChange}
                            onBlur={onBlur}
                        />
                        <FormField
                            name="additionalCostsWithoutVAT"
                            value={form['additionalCostsWithoutVAT']}
                            type={NUMBER_TYPE}
                            settings={settings['additionalCostsWithoutVAT']}
                            onChange={onChange}
                            onBlur={onBlur}
                        />
                        <FormField
                            name="returnCostWithoutVAT"
                            value={form['returnCostWithoutVAT']}
                            type={NUMBER_TYPE}
                            settings={settings['returnCostWithoutVAT']}
                            onChange={onChange}
                            onBlur={onBlur}
                        />
                        {/* <Text
                            name="invoiceNumber"
                            value={form['invoiceNumber']}
                            onChange={onChange}
                        />
                        <Text
                            name="invoiceAmountWithoutVAT"
                            value={form['invoiceAmountWithoutVAT']}
                            onChange={onChange}
                        />*/}
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="additionalCostsComments"
                            rows={10}
                            type={TEXT_TYPE}
                            settings={settings['additionalCostsComments']}
                            value={form['additionalCostsComments']}
                            onChange={onChange}
                            onBlur={onBlur}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('reconciliation of expenses')}</label>
                            <Segment style={{ height: 'calc(100% - 22px)' }}>
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <FormField
                                                checked={form['costsConfirmedByCarrier']}
                                                name="costsConfirmedByCarrier"
                                                type={CHECKBOX_TYPE}
                                                settings={settings['costsConfirmedByCarrier']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                checked={form['costsConfirmedByShipper']}
                                                type={CHECKBOX_TYPE}
                                                settings={settings['costsConfirmedByShipper']}
                                                name="costsConfirmedByShipper"
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        </Form>
    );
};

export default Accounts;
