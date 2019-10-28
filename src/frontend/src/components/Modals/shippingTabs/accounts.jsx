import React from 'react';
import { Checkbox, Form, Grid, Segment } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';
import Text from '../../BaseComponents/Text';
import TextArea from '../../BaseComponents/TextArea';
import Number from '../../BaseComponents/Number';

const Accounts = ({ form = {}, onChange }) => {
    const { t } = useTranslation();

    return (
        <Form>
            <Grid>
                <Grid.Row columns={2} stretched>
                    <Grid.Column>
                        <Number
                            name="deliveryCostWithoutVAT"
                            value={form['deliveryCostWithoutVAT']}
                            onChange={onChange}
                        />
                        <Number
                            name="additionalCostsWithoutVAT"
                            value={form['additionalCostsWithoutVAT']}
                            onChange={onChange}
                        />
                        <Number
                            name="returnCostWithoutVAT"
                            value={form['returnCostWithoutVAT']}
                            onChange={onChange}
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
                        <TextArea
                            name="additionalCostsComments"
                            rows={10}
                            value={form['additionalCostsComments']}
                            onChange={onChange}
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
                                            <Checkbox
                                                checked={form['costsConfirmedByCarrier']}
                                                label={t('costsConfirmedByCarrier')}
                                                name="costsConfirmedByCarrier"
                                                onClick={(event, { name, checked }) =>
                                                    onChange(event, { name, value: checked })
                                                }
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Checkbox
                                                checked={form['costsConfirmedByShipper']}
                                                label={t('costsConfirmedByShipper')}
                                                name="costsConfirmedByShipper"
                                                onClick={(event, { name, checked }) =>
                                                    onChange(event, { name, value: checked })
                                                }
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
