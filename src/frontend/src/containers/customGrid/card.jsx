import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withTranslation } from 'react-i18next';

import { Button, Dimmer, Loader, Modal, Tab } from 'semantic-ui-react';
import { openGridCardRequest, tabsSelector, titleCardSelector } from '../../ducks/gridCard';
import TabCard from "./components/tab";

class Card extends Component {
    state = {
        modalOpen: false,
    };

    loadCard = () => {
        console.log('loadcard', this.props);
        const { openCard, name } = this.props;

        openCard({
            name,
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

    handleSave = () => {};

    getPanes = () => {
        const { tabs = [], t } = this.props;

        console.log('tabs', tabs);

        return tabs.map(tab => ({ menuItem: t(tab), render: () => <Tab.Pane><TabCard name={tab} /></Tab.Pane> }))
    };

    render() {
        const { title, loading, children, progress, tabs, t } = this.props;
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
                <Modal.Header>{t(title, { number: '11' })}</Modal.Header>
                <Modal.Content>
                    <Dimmer active={loading} inverted>
                        <Loader size="huge">Loading</Loader>
                    </Dimmer>
                    <Modal.Description>
                        <Tab panes={this.getPanes()} />
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
    return {
        title: titleCardSelector(state),
        tabs: tabsSelector(state),
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
