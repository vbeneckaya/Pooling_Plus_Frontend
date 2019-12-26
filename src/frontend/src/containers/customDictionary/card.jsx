import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withTranslation } from 'react-i18next';
import { Button, Confirm, Dimmer, Loader, Modal } from 'semantic-ui-react';
import {
    canDeleteSelector,
    cardProgressSelector,
    cardSelector,
    clearDictionaryCard,
    clearDictionaryInfo,
    columnsSelector,
    deleteDictionaryEntryRequest,
    errorSelector,
    getCardRequest,
    saveDictionaryCardRequest,
} from '../../ducks/dictionaryView';
import FormField from '../../components/BaseComponents';

const initialState = {
    modalOpen: false,
    form: {},
    confirmation: { open: false },
    notChangeForm: true,
};

class Card extends Component {
    constructor(props = {}) {
        //console.log('props.defaultForm', props.defaultForm);
        super(props);

        this.state = {
            ...initialState,
            form: {
                ...props.defaultForm,
            },
        };
    }

    componentDidUpdate(prevProps) {
        if (this.props.defaultForm !== prevProps.defaultForm) {
            this.setState(prevState => ({
                form: {
                    ...prevState.form,
                    ...this.props.defaultForm,
                },
            }));
        }

        if (prevProps.card !== this.props.card) {
            this.setState({
                form: {
                    ...this.props.card,
                },
            });
        }
    }

    loadCard = () => {
        const { id, getCard, name } = this.props;

        id && getCard({ id, name });
    };

    onOpen = () => {
        this.loadCard();
        this.setState({
            modalOpen: true,
        });
    };

    confirmClose = () => {
        const {loadList, clearCard} = this.props;

        this.setState({
            ...initialState,
        });
        clearCard();
        loadList && loadList(false, true);
    };

    onClose = () => {
        const { notChangeForm } = this.state;
        const { t } = this.props;

        if (notChangeForm) {
            this.confirmClose();
        } else {
            this.setState({
                confirmation: {
                    open: true,
                    content: t('confirm_close_dictionary'),
                    onCancel: () => {
                        this.setState({
                            confirmation: { open: false },
                        });
                    },
                    onConfirm: this.confirmClose,
                },
            });
        }
    };

    handleChange = (event, { name, value }) => {
        this.setState(prevState => ({
            notChangeForm: false,
            form: {
                ...prevState.form,
                [name]: value,
            },
        }));
    };

    handleSave = () => {
        const {id, save, name, load} = this.props;
        const { form } = this.state;

        let params = {
            ...form,
        };

        if (id) {
            params = {
                ...params,
                id,
            };
        }

        save({
            params,
            name,
            callbackSuccess: () => {
                load && load(form);
                this.confirmClose();
            },
        });
    };

    handleDelete = () => {
        const {id, deleteEntry, name} = this.props;

        deleteEntry({
            name,
            id,
            callbackSuccess: this.confirmClose,
        });
    };

    render() {
        const {title, loading, children, progress, columns, t, error, canDelete} = this.props;
        const { modalOpen, form, confirmation } = this.state;
        //console.log('column', columns, form);
        return (
            <Modal
                dimmer="blurring"
                trigger={children}
                open={modalOpen}
                closeOnDimmerClick={false}
                onOpen={this.onOpen}
                onClose={this.onClose}
                closeIcon
            >
                <Modal.Header>{title}</Modal.Header>
                <Modal.Content scrolling>
                    <Dimmer active={loading} inverted>
                        <Loader size="huge">Loading</Loader>
                    </Dimmer>
                    <Modal.Description>
                        <div className="ui form dictionary-edit">
                            {columns.map(column => {
                                return (
                                    <FormField
                                        {...column}
                                        noScrollColumn={column}
                                        key={column.name}
                                        error={error[column.name]}
                                        value={form[column.name]}
                                        onChange={this.handleChange}
                                    />
                                );
                            })}
                        </div>
                    </Modal.Description>
                </Modal.Content>
                <Modal.Actions className="grid-card-actions">
                    <div>
                        {canDelete && form.id ? (
                            <Button color="red" onClick={this.handleDelete}>
                                {t('delete')}
                            </Button>
                        ) : null}
                    </div>
                    <div>
                        <Button color="grey" onClick={this.onClose}>
                            {t('CancelButton')}
                        </Button>
                        <Button color="blue" loading={progress} onClick={this.handleSave}>
                            {t('SaveButton')}
                        </Button>
                    </div>
                </Modal.Actions>
                <Confirm
                    dimmer="blurring"
                    open={confirmation.open}
                    onCancel={confirmation.onCancel}
                    cancelButton={t('cancelConfirm')}
                    onConfirm={confirmation.onConfirm}
                    content={confirmation.content}
                />
            </Modal>
        );
    }
}

const mapStateToProps = (state, ownProps) => {
    const { name } = ownProps;

    return {
        columns: ownProps.columns ? ownProps.columns : columnsSelector(state, name),
        canDelete: canDeleteSelector(state, name),
        loading: cardProgressSelector(state),
        card: cardSelector(state),
        error: errorSelector(state),
    };
};

const mapDispatchToProps = dispatch => {
    return {
        getCard: params => {
            dispatch(getCardRequest(params));
        },
        save: params => {
            dispatch(saveDictionaryCardRequest(params));
        },
        clearCard: () => {
            dispatch(clearDictionaryCard());
        },
        deleteEntry: params => {
            dispatch(deleteDictionaryEntryRequest(params));
        },
    };
};

export default withTranslation()(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(Card),
);
