import React, {Component} from 'react';
import {connect} from 'react-redux';

import {Button, Dimmer, Form, Grid, Input, Label, Loader, Modal} from 'semantic-ui-react';
import {
    clearUsersInfo,
    createUserRequest,
    getUserCardRequest,
    progressSelector,
    userCardSelector,
} from '../../ducks/users';
import {getRolesRequest, progressSelector as rolesProgressSelector, rolesFromUserSelector,} from '../../ducks/roles';
import Select from '../../components/BaseComponents/Select';

class UserCard extends Component {
    constructor(props) {
        super(props);

        this.state = {
            modalOpen: false,
            form: {
                login: null,
                name: null,
                roleId: null,
                email: null,
                isActive: true,
            },
        };
    }

    componentDidUpdate(prevProps) {
        if (prevProps.user !== this.props.user) {
            const {user = {}} = this.props;

            this.setState({
                form: {
                    login: user.login,
                    name: user.name,
                    roleId: user.roleId,
                    email: user.email,
                    password: user.password,
                    isActive: user.isActive,
                },
            });
        }
    }

    handleOpen = () => {
        const {getUser, id, getRoles} = this.props;

        id && getUser(id);
        getRoles({
            filter: {
                skip: 0,
                take: 20,
            },
        });
        this.setState({modalOpen: true});
    };

    handleClose = () => {
        const {loadList, clear} = this.props;

        this.setState({modalOpen: false});

        clear();
        loadList();
    };

    handleChange = (event, {name, value}) => {
        this.setState(prevState => ({
            form: {
                ...prevState.form,
                [name]: value,
            },
        }));
    };

    mapProps = () => {
        const {form} = this.state;
        const {id} = this.props;

        let params = {...form};

        if (id) {
            params = {
                ...params,
                id,
            };
        }

        return params;
    };

    handleCreate = () => {
        const {createUser} = this.props;

        createUser({params: this.mapProps(), callbackFunc: this.handleClose});
    };

    render() {
        const {modalOpen, form} = this.state;
        const {login, name, roleId, email, isActive} = form;
        const {children, title, loading, id, rolesLoading, roles} = this.props;

        console.log('roles', roles);

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
                                        <Select
                                            fluid
                                            search
                                            selection
                                            loading={rolesLoading}
                                            name="roleId"
                                            value={roleId}
                                            valuesList={roles}
                                            onChange={this.handleChange}
                                        />
                                    </Form.Field>
                                    <Form.Field>
                                        <label>Пароль</label>
                                        <Input
                                            type="password"
                                            name="password"
                                            onChange={this.handleChange}
                                        />
                                        {id ? (
                                            <Label pointing>
                                                Оставьте поле пустым, если не хотите менять пароль
                                            </Label>
                                        ) : null}
                                    </Form.Field>
                                    <Form.Field>
                                        <Form.Checkbox
                                            label="Активен"
                                            name="isActive"
                                            checked={isActive}
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
        roles: rolesFromUserSelector(state),
        rolesLoading: rolesProgressSelector(state),
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
            dispatch(clearUsersInfo());
        },
    };
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(UserCard);
