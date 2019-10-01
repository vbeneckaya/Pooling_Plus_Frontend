import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid, Segment } from 'semantic-ui-react';
import Text from '../../BaseComponents/Text';
import State from '../../BaseComponents/State';
import Date from '../../BaseComponents/Date';
import Select from '../../BaseComponents/Select';
import TextArea from '../../BaseComponents/TextArea';

const Information = ({ form, onChange }) => {
    console.log('form', form);

    const { t } = useTranslation();

    return (
        <Form>
            <Grid>
                <Grid.Row columns={5}>
                    <Grid.Column>
                        <Text
                            name="orderNumber"
                            value={form['orderNumber']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <State name="status" value={form['status']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Date name="createAt" value={form['createAt']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Select name="orderType" value={form['orderType']} onChange={onChange} />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={5}>
                    <Grid.Column>
                        <Text name="payer" value={form['payer']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Text name="client" value={form['client']} isDisabled onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Text name="soldTo" value={form['soldTo']} isDisabled onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Select
                            name="complectationType"
                            value={form['complectationType']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Select name="termType" value={form['termType']} onChange={onChange} />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('route')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <TextArea
                                                name="addressFrom"
                                                value={form['addressFrom']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <TextArea
                                                name="addressTo"
                                                value={form['addressTo']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('palletsCountGroup')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={3}>
                                        <Grid.Column>
                                            <Text
                                                name="prepare"
                                                value={form['prepare']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Text
                                                name="plan"
                                                value={form['plan']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Text
                                                name="fact"
                                                value={form['fact']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('boxesCountGroup')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <Text
                                                name="prepare"
                                                value={form['prepare']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Text
                                                name="fact"
                                                value={form['fact']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('weigth')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <Text
                                                name="prepare"
                                                value={form['prepare']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Text
                                                name="fact"
                                                value={form['fact']}
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

export default Information;
