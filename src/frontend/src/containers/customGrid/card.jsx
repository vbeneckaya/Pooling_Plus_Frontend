import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withTranslation } from 'react-i18next';

import { Button, Dimmer, Loader, Modal } from 'semantic-ui-react';
import { cardSelector, openGridCardRequest, progressSelector } from '../../ducks/gridCard';
import OrderModal from '../../components/Modals/orderModal';
import ShippingModal from "../../components/Modals/shippingModal";

const getModal = {
    orders: <OrderModal />,
    shippings: <ShippingModal />,
};

class Card extends Component {
    state = {
        modalOpen: false,
        form: {},
    };

    loadCard = () => {
        const { openCard, name, id } = this.props;

        console.log('___id', id);

        openCard({
            name,
            id,
            callbackSuccess: (card) => {
                this.setState({
                    form: card,
                });
            }
        });
    };

    onOpen = () => {
        this.loadCard();
        this.props.stopUpdate();
        this.setState({
            modalOpen: true,
        });
    };

    onClose = () => {
        const { loadList } = this.props;

        this.setState({
            modalOpen: false,
        });
        loadList(false, true);
    };

    onChangeForm = (e, { name, value }) => {
        this.setState(prevState => ({
            form: {
                ...prevState.form,
                [name]: value,
            },
        }));
    };

    handleSave = () => {};

    render() {
        const { title, loading, children, progress, name, t } = this.props;
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
                size="fullscreen"
            >
                <Modal.Header>{t(title)}</Modal.Header>
                <Modal.Content scrolling>
                    <Dimmer active={loading} inverted>
                        <Loader size="huge">Loading</Loader>
                    </Dimmer>
                    <Modal.Description>
                        {React.cloneElement(getModal[name], {
                            ...this.props,
                            form,
                            onChangeForm: this.onChangeForm,
                        })}
                    </Modal.Description>
                </Modal.Content>
                <Modal.Actions>
                    <Button color="grey" onClick={this.onClose}>
                        Отмена
                    </Button>
                    <Button color="blue" loading={progress} onClick={this.handleSave}>
                        Сохранить
                    </Button>
                </Modal.Actions>
            </Modal>
        );
    }
}

const mapStateToProps = (state, ownProps) => {
    return {
        card: cardSelector(state),
        loading: progressSelector(state),
    };
};

const mapDispatchToProps = dispatch => {
    return {
        openCard: params => {
            dispatch(openGridCardRequest(params));
        },
    };
};

export default withTranslation()(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(Card),
);
