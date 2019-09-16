import React, { Component } from 'react';
import {Button, Dimmer, Loader, Modal} from 'semantic-ui-react';

class Card extends Component {
    state = {
        modalOpen: false,
    };

    loadCard = () => {};

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

    handleSave = () => {

    };

    render() {
        const { title, loading, children, progress } = this.props;
        const { modalOpen } = this.state;

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
                <Modal.Header>
                    {title}
                </Modal.Header>
                <Modal.Content>
                    <Dimmer active={loading} inverted>
                        <Loader size="huge">Loading</Loader>
                    </Dimmer>
                    <Modal.Description>
                    </Modal.Description>
                </Modal.Content>
                <Modal.Actions>
                    <Button
                        color="blue"
                        loading={progress}
                        onClick={this.handleSave}
                    >
                        Сохранить
                    </Button>
                    <Button color="grey" onClick={this.onClose}>
                        Отмена
                    </Button>
                </Modal.Actions>
        </Modal>
        );
    }
}

export default Card;
