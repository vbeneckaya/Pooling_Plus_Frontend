import React from 'react';
import { Checkbox, Form, Grid, Segment } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';
import Text from '../../BaseComponents/Text';
import TextArea from '../../BaseComponents/TextArea';

const Accounts = ({ form = {}, onChange }) => {
    const { t } = useTranslation();

    return (
        <Form>
            <Grid>
                <Grid.Row columns={2} stretched>
                    <Grid.Column>
                        <Text
                            name="transportationCostWithoutVAT"
                            value={form['transportationCostWithoutVAT']}
                            onChange={onChange}
                        />
                        <Text
                            name="additionalShippingCostsExcludingVAT"
                            value={form['additionalShippingCostsExcludingVAT']}
                            onChange={onChange}
                        />
                        <Text
                            name="returnShippingCostExcludingVAT"
                            value={form['returnShippingCostExcludingVAT']}
                            onChange={onChange}
                        />
                        <Text
                            name="invoiceNumber"
                            value={form['invoiceNumber']}
                            onChange={onChange}
                        />
                        <Text

                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <TextArea
                            name="additionalShippingCostsComments"
                            rows={5}
                            value={form['additionalShippingCostsComments']}
                            onChange={onChange}
                        />
                        <Form.Field>
                            <label>{t('reconciliation of expenses')}</label>
                            <Segment style={{height: "calc(100% - 22px)"}}>
                                <Grid>
                                    <Grid.Row columns={1} stretched>
                                        <Grid.Column>
                                            <Checkbox
                                                checked={form['amountConfirmedByShipper']}
                                                label={t('Расходы подтверждены грузоотправителем')}
                                                onChange={onChange}
                                            />
                                            <Checkbox
                                                checked={form['amountConfirmedByTC']}
                                                label={t('Расходы подтверждены ТК')}
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
