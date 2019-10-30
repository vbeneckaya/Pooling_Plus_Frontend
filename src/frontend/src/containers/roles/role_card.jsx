import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withTranslation } from 'react-i18next';
import { Button, Form, Input, Modal, Dimmer, Loader } from 'semantic-ui-react';
import {
    allPermissionsSelector,
    clearRolesInfo,
    createRoleRequest,
    getAllPermissionsRequest,
    getRoleCardRequest,
    progressSelector,
    roleCardSelector,
} from '../../ducks/roles';

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
            const { permissions = [] } = role;

            this.setState({
                form: {
                    ...role,
                    permissions: permissions.map(item => item.code),
                },
            });
        }
    }

    handleOpen = () => {
        const { getRole, id, getAllPermissions } = this.props;

        id && getRole(id);
        getAllPermissions && getAllPermissions();

        this.setState({
            modalOpen: true,
        });
    };

    handleClose = () => {
        const { loadList, clear } = this.props;

        this.setState({
            modalOpen: false,
        });

        this.setState({ form: {} });

        clear();

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

        if (value === 1 && !selectedPermissions.has(value)) {
            selectedPermissions.delete(2);
            selectedPermissions.delete(4);
            selectedPermissions.delete(5);
            selectedPermissions.delete(6);
        }

        if (value === 7 && !selectedPermissions.has(value)) {
            selectedPermissions.delete(10);
            selectedPermissions.delete(11);
            selectedPermissions.delete(12);
        }

        this.handleChange(null, { name: 'permissions', value: Array.from(selectedPermissions) });
    };

    mapData = () => {
        const { form } = this.state;

        return {
            ...form,
            permissions: form.permissions.map(item => ({
                code: item,
            })),
        };
    };

    handleCreate = () => {
        const { createRole } = this.props;

        createRole({ params: this.mapData(), callbackFunc: this.handleClose });
    };

    render() {
        const { title, children, loading, t, allPermissions } = this.props;
        const { modalOpen, form } = this.state;
        const { name, permissions = [] } = form;

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
                            <label>{t('name')}</label>
                            <Input value={name} name="name" onChange={this.handleChange} />
                        </Form.Field>
                        <Form.Field>
                            <label>{t('permissions')}</label>
                        </Form.Field>
                        {allPermissions.map(permission => (
                            <Form.Field key={permission.code}>
                                <Form.Checkbox
                                    label={t(permission.name)}
                                    value={permission.code}
                                    checked={permissions.includes(permission.code)}
                                    disabled={([2, 4, 5, 6].includes(permission.code) && !permissions.includes(1)) || ([10, 11, 12].includes(permission.code) && !permissions.includes(7))}
                                    onChange={this.handlePermissions}
                                />
                            </Form.Field>
                        ))}
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
        role: roleCardSelector(state),
        loading: progressSelector(state),
        allPermissions: allPermissionsSelector(state),
    };
};

const mapDispatchToProps = dispatch => {
    return {
        getRole: params => {
            dispatch(getRoleCardRequest(params));
        },
        createRole: params => {
            dispatch(createRoleRequest(params));
        },
        clear: () => {
            dispatch(clearRolesInfo());
        },
        getAllPermissions: () => {
            dispatch(getAllPermissionsRequest());
        },
    };
};

export default withTranslation()(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(RoleCard),
);
