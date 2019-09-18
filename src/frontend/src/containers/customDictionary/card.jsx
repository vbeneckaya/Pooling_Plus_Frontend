import React, { Component } from 'react';
import {connect} from 'react-redux';
import { Button, Dimmer, Loader, Modal } from 'semantic-ui-react';
import {columnsSelector} from "../../ducks/dictionaryView";
import FormField from "../../components/BaseComponents";

class Card extends Component {
    state = {
        modalOpen: false,
    };

    loadCard = () => {};

    onOpen = () => {
        this.loadCard();
        this.setState({
            modalOpen: true,
        });
    };

    onClose = () => {
        console.log('tt', this.props)
        const { loadList } = this.props;

        this.setState({
            modalOpen: false,
        });
        loadList(false, true);
    };

    handleSave = () => {};

    render() {
        const { title, loading, children, progress, columns } = this.props;
        const { modalOpen } = this.state;

        console.log('columns', columns);

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
                <Modal.Header>{title}</Modal.Header>
                <Modal.Content scrolling>
                    <Dimmer active={loading} inverted>
                        <Loader size="huge">Loading</Loader>
                    </Dimmer>
                    <Modal.Description>
                        <div className="ui form dictionary-edit">
                            {
                                columns.map(column => {
                                    return(
                                        <FormField column={column} />
                                    )
                                })
                            }
                        </div>
                    </Modal.Description>
                </Modal.Content>
                <Modal.Actions>
                    <Button color="blue" loading={progress} onClick={this.handleSave}>
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

const mapStateToProps = (state, ownProps) => {
    const {name} = ownProps;
    console.log('__', ownProps);

    return {
        columns: columnsSelector(state, name),
    }
};

const mapDispatchToProps = (dispatch) => {
    return {}
};

export default connect(mapStateToProps, mapDispatchToProps)(Card);
