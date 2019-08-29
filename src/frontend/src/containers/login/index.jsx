import React, { useEffect, useState } from 'react';

import { Grid, Button, Input, Form, Dropdown, Flag, Dimmer, Loader } from 'semantic-ui-react';

import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import {
    errorSelector,
    getLoginPageRequest,
    loginPageSelector,
    loginRequest,
    progressSelector,
} from '../../ducks/login';
import languages from '../../constants/languages';
import './style.scss';

const Login = () => {
    const { t, i18n } = useTranslation();
    const dispatch = useDispatch();
    const page = useSelector(state => loginPageSelector(state));
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
            form,
            api: login_btn.api,
        };
        dispatch(loginRequest(params));
    };

    return (
        <>
            <Dimmer inverted active={pageLoadingProgress}>
                <Loader inverted>Loading</Loader>
            </Dimmer>
            <div className="centered-div">
                <Grid className="login-form-wrapper">
                    <Grid.Row>
                        <Grid.Column className="login-form-description">
                            <div>
                                <img src={page.logo} />
                            </div>
                            <div>
                                <p>{t(page.name)}</p>
                                <p>
                                    {t(page.support_name)}: <a>{page.phoneNumber}</a>
                                </p>
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
                                <Button floated="right" api={login_btn.api} loading={loginProgress}>
                                    {t(login_btn.name)}
                                </Button>
                                <div className="error">{error}</div>
                            </Form>
                        </Grid.Column>
                    </Grid.Row>
                </Grid>
            </div>
            <div className="language-switcher">
                <Flag name={languages.find(item => item.value === i18n.language).flag} />
                <span>{t('Language')}:</span>{' '}
                <Dropdown
                    inline
                    options={languages}
                    value={i18n.language}
                    onChange={handleChangeLang}
                />
            </div>
        </>
    );
};

export default Login;
