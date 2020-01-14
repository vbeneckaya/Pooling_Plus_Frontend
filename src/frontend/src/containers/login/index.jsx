import React, { useEffect, useState } from 'react';

import { Button, Dimmer, Dropdown, Flag, Form, Grid, Input, Loader, List } from 'semantic-ui-react';

import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import {
    errorSelector,
    getLoginPageRequest,
    isAuthSelector,
    loginPageSelector,
    loginRequest,
    progressSelector,
} from '../../ducks/login';
import languages from '../../constants/languages';
import './style.scss';

const Login = () => {
    const { t, i18n } = useTranslation();
    const dispatch = useDispatch();
    const page = useSelector(state => loginPageSelector(state)) || {};
    const isAuth = useSelector(state => isAuthSelector(state));
    const error = useSelector(state => errorSelector(state));
    const loginProgress = useSelector(state => progressSelector(state, 'login_progress'));
    const pageLoadingProgress = useSelector(state => progressSelector(state, 'page_progress'));
    const { form: formElements = {} } = page;
    const { inputs = [], login_btn = {} } = formElements;

    const [form, setForm] = useState({});

    useEffect(() => {
        dispatch(getLoginPageRequest());
    }, []);

    const handleChange = (e, { name, value }) => {
        const values = {
            ...form,
            [name]: value,
        };
        setForm(values);
    };

    const handleChangeLang = (e, { value }) => {
        i18n.changeLanguage(value);
    };

    const handleFormSubmit = (e, { api }) => {
        const params = {
            form: {
                ...form,
                language: i18n.language,
            },
            api: login_btn.api,
        };
        dispatch(loginRequest(params));
    };

    return (
        <>
            <Dimmer inverted active={pageLoadingProgress}>
                <Loader inverted>Loading</Loader>
            </Dimmer>
            {!isAuth ? (
                <>
                    <img src={'/main-logo.png'} alt={'LOGO'}  className="main-logo" />
                    <div className="centered-div">
                        <Grid className="login-form-wrapper">
                            <Grid.Row>
                                <Grid.Column className="login-form-description">
                                    <div className="login-form-description-wrapper">
										<img src={'/poolingPlus.png'} alt={'LOGO'} />
										<List bulleted>
											<List.Item>{t("login_feature1")}</List.Item>
											<List.Item>{t("login_feature2")}</List.Item>
											<List.Item>{t("login_feature3")}</List.Item>
										</List>										
                                        <p><a href="mailto:support@artlogics.ru">support@artlogics.ru</a></p>
                                    </div>
                                </Grid.Column>
                                <Grid.Column className="login-form-input-wrapper">
                                    <Form onSubmit={handleFormSubmit}>
                                        {inputs.map(input => (
                                            <div className="margin-bottom-10" key={input.name}>
                                                <Input
                                                    icon={input.icon}
                                                    iconPosition="left"
                                                    name={input.name}
                                                    value={form[input.name]}
                                                    placeholder={t(input.name)}
                                                    type={input.type}
                                                    onChange={handleChange}
                                                />
                                            </div>
                                        ))}
                                        <Button
                                            floated="right"
                                            primary
                                            api={login_btn.api}
                                            loading={loginProgress}
                                        >
                                            {t(login_btn.name)}
                                        </Button>
                                        <div className="error">{t(error)}</div>
                                    </Form>
                                </Grid.Column>
                            </Grid.Row>
                        </Grid>
                    </div>
                    <div className="language-switcher">
                        <Flag
                            name={
                                languages.find(item => item.value === i18n.language) &&
                                languages.find(item => item.value === i18n.language).flag
                            }
                        />{' '}
                        <Dropdown
                            inline
                            options={languages}
                            value={i18n.language}
                            onChange={handleChangeLang}
                        />
                    </div>
                </>
            ) : null}
        </>
    );
};

export default Login;
