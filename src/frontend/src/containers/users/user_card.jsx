import React, { Component } from 'react';
import { connect } from 'react-redux';

import { Button, Dimmer, Form, Grid, Input, Loader, Modal } from 'semantic-ui-react';
import {
    clearUsersInfo,
    createUserRequest,
    getUserCardRequest,
    progressSelector,
    userCardSelector,
} from '../../ducks/users';

class UserCard extends Component {
    constructor(props) {
        super(props);

        this.state = {
            modalOpen: false,
            form: {
                login: '',
                name: '',
                role: '',
                email: '',
                is_active: null,
            },
        };
    }

    componentDidUpdate(prevProps) {
        if (prevProps.user !== this.props.user) {
            const { user = {} } = this.props;

            this.setState({
                form: {
                    login: user.login,
                    name: user.name,
                    role: user.role,
                    email: user.email,
                    is_active: user.is_active,
                },
            });
        }
    }

    handleOpen = () => {
        const { getUser, id } = this.props;

        getUser(id);
        this.setState({ modalOpen: true });
    };

    handleClose = () => {
        const { loadList, clear } = this.props;

        this.setState({ modalOpen: false });

        clear();
        loadList();
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
        const { login, name, role, email, is_active } = form;
        const { children, title, loading } = this.props;

        return (
            <Modal
                trigger={children}
                open={modalOpen}
                dimmer="blurring"
                className="card-modal"
                closeIcon
                onOpen={this.handleOpen}
                onClose={this.handleClose}
            >
                <Modal.Header>{title}</Modal.Header>
                <Modal.Content scrolling>
                    <Dimmer active={loading} inverted className="table-loader">
                        <Loader size="huge">Loading</Loader>
                    </Dimmer>
                    <Form>
                        <Grid columns="equal">
                            <Grid.Row columns="equal">
                                <Grid.Column>
                                    <Form.Field>
                                        <label>Логин</label>
                                        <Input
                                            name="login"
                                            value={login}
                                            onChange={this.handleChange}
                                        />
                                    </Form.Field>
                                    <Form.Field>
                                        <label>ФИО</label>
                                        <Input
                                            name="name"
                                            value={name}
                                            onChange={this.handleChange}
                                        />
                                    </Form.Field>
                                    <Form.Field>
                                        <label>Email</label>
                                        <Input
                                            name="email"
                                            value={email}
                                            onChange={this.handleChange}
                                        />
                                    </Form.Field>
                                    <Form.Field>
                                        <label>Роль</label>
                                        <Input
                                            name="role"
                                            value={role}
                                            onChange={this.handleChange}
                                        />
                                    </Form.Field>
                                    <Form.Field>
                                        <Form.Checkbox
                                            label="Активен"
                                            name="is_active"
                                            checked={is_active}
                                            onChange={this.handleChange}
                                        />
                                    </Form.Field>
                                </Grid.Column>
                            </Grid.Row>
                        </Grid>
                    </Form>
                </Modal.Content>
                <Modal.Actions>
                    <Button onClick={this.handleClose}>Отменить</Button>
                    <Button color="blue" onClick={this.handleCreate}>
                        Сохранить
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
    };
};

const mapDispatchToProps = dispatch => {
    return {
        getUser: params => {
            dispatch(getUserCardRequest(params));
        },
        createUser: params => {
            dispatch(createUserRequest(params));
        },
        clear: () => {
            dispatch(clearUsersInfo());
        },
    };
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(UserCard);
