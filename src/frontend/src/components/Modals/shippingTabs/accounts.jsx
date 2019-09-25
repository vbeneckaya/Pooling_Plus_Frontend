import React from 'react';
import {Form, Grid, Segment} from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';
import Text from '../../BaseComponents/Text';
import TextArea from '../../BaseComponents/TextArea';

const Accounts = ({ form = {}, onChange }) => {
    const { t } = useTranslation();

    return (
        <Form>
            <Grid>
                <Grid.Row columns={4}>
                    <Grid.Column>
                        <Text
                            name="invoiceNumber"
                            value={form['invoiceNumber']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Text
                            name="transportationCostWithoutVAT"
                            value={form['transportationCostWithoutVAT']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Text
                            name="additionalShippingCostsExcludingVAT"
                            value={form['additionalShippingCostsExcludingVAT']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Text
                            name="returnShippingCostExcludingVAT"
                            value={form['returnShippingCostExcludingVAT']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row stretched columns='equal'>
                    <Grid.Column width={6}>
                        <Text
                            name="amount"
                            value={form['amount']}
                            onChange={onChange}
                        />
                        <TextArea
                            name="comment"
                            value={form['comment']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('costExcluding')}</label>
                        <Segment>
                            <Grid>
                                <Grid.Row columns={3}>
                                    <Grid.Column>
                                        <Text
                                            name="amount"
                                            value={form['amount']}
                                            onChange={onChange}
                                        />
                                    </Grid.Column>
                                    <Grid.Column>
                                        <Text
                                            name="amount"
                                            value={form['amount']}
                                            onChange={onChange}
                                        />
                                    </Grid.Column>
                                    <Grid.Column>
                                        <Text
                                            name="amount"
                                            value={form['amount']}
                                            onChange={onChange}
                                        />
                                    </Grid.Column>
                                </Grid.Row>
                                <Grid.Row columns={3}>
                                    <Grid.Column>
                                        <Text
                                            name="amount"
                                            value={form['amount']}
                                            onChange={onChange}
                                        />
                                    </Grid.Column>
                                    <Grid.Column>
                                        <Text
                                            name="amount"
                                            value={form['amount']}
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
