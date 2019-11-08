import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withTranslation } from 'react-i18next';
import { Button, Dimmer, Loader, Modal } from 'semantic-ui-react';
import {
    cardSelector,
    clearDictionaryInfo,
    columnsSelector,
    getCardRequest,
    saveDictionaryCardRequest,
} from '../../ducks/dictionaryView';
import FormField from '../../components/BaseComponents';

const initialState = {
    modalOpen: false,
    form: {},
};

class Card extends Component {
    state = {
        ...initialState,
    };

    componentDidUpdate(prevProps) {
        if (prevProps.card !== this.props.card) {
            const { user = {} } = this.props;

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

    onClose = () => {
        const { loadList, clear } = this.props;

        this.setState({
            ...initialState,
        });
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

    handleSave = () => {
        const { id, save, name } = this.props;
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
            callbackSuccess: this.onClose,
        });
    };

    render() {
        const { title, loading, children, progress, columns, t } = this.props;
        const { modalOpen, form } = this.state;

        return (
            <Modal
                dimmer="blurring"
                className="card-modal"
                trigger={children}
                open={modalOpen}
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
                                        column={column}
                                        key={column.name}
                                        value={form[column.name]}
                                        onChange={this.handleChange}
                                    />
                                );
                            })}
                        </div>
                    </Modal.Description>
                </Modal.Content>
                <Modal.Actions>
                    <Button color="grey" onClick={this.onClose}>
                        {t('CancelButton')}
                    </Button>
                    <Button color="blue" loading={progress} onClick={this.handleSave}>
                        {t('SaveButton')}
                    </Button>
                </Modal.Actions>
            </Modal>
        );
    }
}

const mapStateToProps = (state, ownProps) => {
    const { name } = ownProps;

    return {
        columns: columnsSelector(state, name),
        card: cardSelector(state),
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
        clear: () => {
            dispatch(clearDictionaryInfo());
        },
    };
};

export default withTranslation()(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(Card),
);
