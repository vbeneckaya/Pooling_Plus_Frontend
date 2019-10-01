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
                <Grid.Row columns={4}>
                    <Grid.Column>
                        <Text
                            name="orderNumber"
                            value={form['orderNumber']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Text name="payer" value={form['payer']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Text name="clientName" value={form['clientName']} isDisabled onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Select
                            name="pickingType"
                            value={form['pickingType']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={4}>
                    <Grid.Column>
                        <Date name="orderCreationDate" value={form['orderCreationDate']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Select name="orderType" value={form['orderType']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Text name="soldTo" value={form['soldTo']} isDisabled onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Select name="temperature" value={form['temperature']} onChange={onChange} />
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
                                                rows={2}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <TextArea
                                                name="addressTo"
                                                value={form['addressTo']}
                                                rows={2}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={3} stretched>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('palletsCountGroup')}</label>
                            <Segment>
                                <Text
                                    name="prepare"
                                    value={form['prepare']}
                                    onChange={onChange}
                                />
                                <Text
                                    name="plan"
                                    value={form['plan']}
                                    onChange={onChange}
                                />
                                <Text
                                    name="fact"
                                    value={form['fact']}
                                    onChange={onChange}
                                />
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('boxesCountGroup')}</label>
                            <Segment style={{height: "calc(100% - 22px)"}}>
                                <Text
                                    name="prepare"
                                    value={form['prepare']}
                                    onChange={onChange}
                                />
                                <Text
                                    name="fact"
                                    value={form['fact']}
                                    onChange={onChange}
                                />
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('weigth')}</label>
                            <Segment style={{height: "calc(100% - 22px)"}}>
                                <Text
                                    name="prepare"
                                    value={form['prepare']}
                                    onChange={onChange}
                                />
                                <Text
                                    name="fact"
                                    value={form['fact']}
                                    onChange={onChange}
                                />
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        </Form>
    );
};

export default Information;
