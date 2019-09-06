import React, {Component} from 'react';
import {connect} from 'react-redux';

import {Button, Dimmer, Form, Grid, Input, Loader, Modal} from 'semantic-ui-react';
import {getUserCardRequest, progressSelector, userCardSelector} from '../../ducks/users';

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
                shippers: null,
                is_active: null,
                regions: null,
                psg: null,
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
                    shippers: user.shippers,
                    is_active: user.is_active,
                    regions: user.regions,
                    psg: user.psg,
                },
            });
        }
    }

    handleOpen = () => {
        const { getUser, id, getShippersList, getDelivery } = this.props;

        getUser(id);
        getShippersList();
        getDelivery({ name: 'rc' });
        getDelivery({ name: 'psg' });
        this.setState({ modalOpen: true });
    };

    handleClose = () => {
        const {loadList} = this.props;

        this.setState({ modalOpen: false });
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

    render() {
        const { modalOpen, form } = this.state;
        const { login, name, role, email, is_active} = form;
        const {
            children,
            title,
            loading,
        } = this.props;

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
                                        <Input name="login" value={login} disabled />
                                    </Form.Field>
                                    <Form.Field>
                                        <label>ФИО</label>
                                        <Input name="name" value={name} disabled />
                                    </Form.Field>
                                    <Form.Field>
                                        <label>Email</label>
                                        <Input name="email" value={email} disabled />
                                    </Form.Field>
                                    <Form.Field>
                                        <label>Роль</label>
                                        <Input name="role" value={role} disabled />
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
                    <Button color="blue">Сохранить</Button>
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
        }
    };
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(UserCard);
