import React, { Component } from 'react';
import { connect } from 'react-redux';

import { Button, Form, Input, Modal, Dimmer, Loader } from 'semantic-ui-react';
import { getRoleCardRequest, progressSelector, roleCardSelector } from '../../ducks/roles';

class RoleCard extends Component {
    constructor(props) {
        super(props);

        this.state = {
            modalOpen: false,
            form: {
                name: '',
                permissions: [],
            },
        };
    }

    componentDidUpdate(prevProps) {
        if (prevProps.role !== this.props.role) {
            const { role = {} } = this.props;

            this.setState({
                form: {
                    permissions: role.permissions,
                    name: role.name,
                },
            });
        }
    }

    handleOpen = () => {
        const { getRole, id } = this.props;

        id && getRole(id);

        this.setState({
            modalOpen: true,
        });
    };

    handleClose = () => {
        const { loadList } = this.props;

        this.setState({
            modalOpen: false,
        });

        loadList();
    };

    handleChange = (e, { name, value }) => {
        this.setState(prevState => ({
            form: {
                ...prevState.form,
                [name]: value,
            },
        }));
    };

    handlePermissions = (e, { value }) => {
        const { permissions } = this.state.form;

        const selectedPermissions = new Set(permissions);

        selectedPermissions[selectedPermissions.has(value) ? 'delete' : 'add'](value);

        this.handleChange(null, { name: 'permissions', value: Array.from(selectedPermissions) });
    };

    render() {
        const { title, children, loading } = this.props;
        const { modalOpen, form } = this.state;
        const { name, permissions } = form;

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
                        <Form.Field>
                            <label>Наименование</label>
                            <Input value={name} name="name" onChange={this.handleChange} />
                        </Form.Field>
                        <Form.Field>
                            <label>Разрешения</label>
                        </Form.Field>
                        {/*{allPermissions.map(permission => (
                            <Form.Field key={permission}>
                                <Form.Checkbox
                                    label={permission}
                                    value={permission}
                                    checked={permissions.includes(permission)}
                                    onChange={this.handlePermissions}
                                />
                            </Form.Field>
                        ))}*/}
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
        role: roleCardSelector(state),
        loading: progressSelector(state),
    };
};

const mapDispatchToProps = dispatch => {
    return {
        getRole: params => {
            dispatch(getRoleCardRequest(params));
        },
    };
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(RoleCard);
