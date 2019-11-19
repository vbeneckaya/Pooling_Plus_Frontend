import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withTranslation } from 'react-i18next';
import { Button, Dimmer, Form, Grid, Loader, Modal } from 'semantic-ui-react';
import {
    clearUserCard,
    clearUsersInfo,
    createUserRequest, errorSelector,
    getUserCardRequest,
    progressSelector,
    userCardSelector,
} from '../../ducks/users';
import {
    getRolesRequest,
    progressSelector as rolesProgressSelector,
    rolesFromUserSelector,
} from '../../ducks/roles';
import Select from '../../components/BaseComponents/Select';
import Text from '../../components/BaseComponents/Text';
import {SELECT_TYPE, TEXT_TYPE} from "../../constants/columnTypes";
import FormField from "../../components/BaseComponents";

class UserCard extends Component {
    constructor(props) {
        super(props);

        this.state = {
            modalOpen: false,
            form: {
                login: null,
                userName: null,
                roleId: null,
                email: null,
                isActive: true,
            },
        };
    }

    componentDidUpdate(prevProps) {
        if (prevProps.user !== this.props.user) {
            const { user = {} } = this.props;

            this.setState({
                form: {
                    login: user.login,
                    userName: user.userName,
                    roleId: user.roleId,
                    email: user.email,
                    password: user.password,
                    isActive: user.isActive,
                },
            });
        }
    }

    handleOpen = () => {
        const { getUser, id, getRoles } = this.props;

        id && getUser(id);
        getRoles({
            filter: {
                skip: 0,
                take: 20,
            },
        });
        this.setState({ modalOpen: true });
    };

    handleClose = () => {
        const { loadList, clear } = this.props;

        this.setState({ modalOpen: false });

        this.setState({ form: {} });
        clear();
        loadList(false, true);
    };

    handleChange = (event, { name, value }) => {
        this.setState(prevState => ({
            form: {
                ...prevState.form,
                [name]: value,
            },
        }));
    };

    mapProps = () => {
        const { form } = this.state;
        const { id } = this.props;

        let params = { ...form };

        if (id) {
            params = {
                ...params,
                id,
            };
        }

        return params;
    };

    handleCreate = () => {
        const { createUser } = this.props;

        createUser({ params: this.mapProps(), callbackFunc: this.handleClose });
    };

    render() {
        const { modalOpen, form } = this.state;
        const { login, userName, roleId, email, isActive, password } = form;
        const {children, title, loading, t, error} = this.props;

        return (
            <Modal
                trigger={children}
                open={modalOpen}
                dimmer="blurring"
                closeIcon
                onOpen={this.handleOpen}
                onClose={this.handleClose}
            >
                <Modal.Header>{title}</Modal.Header>
                <Modal.Content>
                    <Dimmer active={loading} inverted className="table-loader">
                        <Loader size="huge">Loading</Loader>
                    </Dimmer>
                    <Form>
                        <Grid columns="equal">
                            <Grid.Row columns="equal">
                                <Grid.Column>
                                    <FormField
                                        type={TEXT_TYPE}
                                        name="email"
                                        value={email}
                                        error={error && (error.find(item => item.name === 'email') || {}).message}
                                        onChange={this.handleChange}
                                    />
                                    <FormField
                                        name="userName"
                                        value={userName}
                                        type={TEXT_TYPE}
                                        error={error && (error.find(item => item.name === 'userName') || {}).message}
                                        onChange={this.handleChange}
                                    />
                                    <FormField
                                        fluid
                                        search
                                        selection
                                        text="role"
                                        name="roleId"
                                        value={roleId}
                                        source="roles"
                                        error={error && (error.find(item => item.name === 'roleId') || {}).message}
                                        type={SELECT_TYPE}
                                        onChange={this.handleChange}
                                    />
                                    <FormField
                                        typeValue="password"
                                        name="password"
                                        value={password}
                                        type={TEXT_TYPE}
                                        error={error && (error.find(item => item.name === 'password') || {}).message}
                                        autoComplete="new-password"
                                        onChange={this.handleChange}
                                    />
                                    {/*{id ? (
                                            <Label pointing>
                                                Оставьте поле пустым, если не хотите менять пароль
                                            </Label>
                                        ) : null}*/}
                                    <Form.Field>
                                        <Form.Checkbox
                                            label={t('isActive')}
                                            name="isActive"
                                            checked={isActive}
                                            onChange={(e, { name, checked }) =>
                                                this.handleChange(e, { name, value: checked })
                                            }
                                        />
                                    </Form.Field>
                                </Grid.Column>
                            </Grid.Row>
                        </Grid>
                    </Form>
                </Modal.Content>
                <Modal.Actions>
                    <Button onClick={this.handleClose}>{t('CancelButton')}</Button>
                    <Button color="blue" onClick={this.handleCreate}>
                        {t('SaveButton')}
                    </Button>
                </Modal.Actions>
            </Modal>
        );
    }
}

const mapStateToProps = state => {
    return {
        user: userCardSelector(state),
        loading: progressSelector(state),
        roles: rolesFromUserSelector(state),
        rolesLoading: rolesProgressSelector(state),
        error: errorSelector(state)
    };
};

const mapDispatchToProps = dispatch => {
    return {
        getUser: params => {
            dispatch(getUserCardRequest(params));
        },
        getRoles: params => {
            dispatch(getRolesRequest(params));
        },
        createUser: params => {
            dispatch(createUserRequest(params));
        },
        clear: () => {
            dispatch(clearUserCard());
        },
    };
};

export default withTranslation()(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(UserCard),
);
