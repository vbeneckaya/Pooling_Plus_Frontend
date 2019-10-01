import React, { Component } from 'react';
import {withTranslation} from 'react-i18next';
import { Modal, Form, Button, Icon, Input } from 'semantic-ui-react';
import Webcam from 'react-webcam';

class DocWithEditor extends Component {
    state = {
        modalOpen: false,
        mode: 'webcam',
        imageSrc: '',
    };

    setRef = webcam => {
        this.webcam = webcam;
    };

    handleOpen = () => {
        this.setState({ modalOpen: true });
    };

    handleClose = () => {
        this.setState({ modalOpen: false });
    };

    takePhoto = () => {
        const imageSrc = this.webcam.getScreenshot();
        this.setState({
            mode: 'image',
            imageSrc: imageSrc,
        });
    };

    rephoto = () => {
        debugger;
        this.setState({
            mode: 'webcam',
            imageSrc: '',
        });
    };

    save = () => {
        // callApi("files/uploadBase64File", {
        //     base64: this.state.imageSrc,
        //     fileName: "WebCamPhoto.jpg"
        // })
        //     .then((fileDto) => {
        //         this.props.onChange(fileDto);
        //         this.setState({
        //             modalOpen: false,
        //             imageSrc: ""
        //         });
        //     });
    };

    render() {
        const { okButtonText, titleText, document, t } = this.props;
        return (
            <Modal
                trigger={this.props.children}
                onOpen={this.handleOpen}
                open={this.state.modalOpen}
                closeOnEscape
                closeOnDimmerClick={false}
                onClose={this.handleClose}
            >
                <Modal.Header>{t('Create a Photo')}</Modal.Header>
                <Modal.Content>
                    {this.state.mode === 'image' ? (
                        <img src={this.state.imageSrc} />
                    ) : (
                        <Webcam ref={this.setRef} />
                    )}
                </Modal.Content>
                <Modal.Actions>
                    <Button color="red" onClick={this.handleClose}>
                        <Icon name="ban" />{t('CancelButton')}
                    </Button>
                    {this.state.mode === 'webcam' ? (
                        <Button color="green" onClick={this.takePhoto}>
                            <Icon name="photo" />{t('PhotoButton')}
                        </Button>
                    ) : (
                        ''
                    )}
                    {this.state.mode === 'image' ? (
                        <Button color="orange" onClick={this.rephoto}>
                            <Icon name="photo" /> Rephoto
                        </Button>
                    ) : (
                        ''
                    )}
                    {this.state.mode === 'image' ? (
                        <Button color="green" onClick={this.save}>
                            <Icon name="save" /> Take it
                        </Button>
                    ) : (
                        ''
                    )}
                </Modal.Actions>
            </Modal>
        );
    }
}

export default withTranslation()(DocWithEditor);
