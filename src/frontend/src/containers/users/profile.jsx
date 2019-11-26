import React, {useState, useEffect} from 'react';
import {useTranslation} from 'react-i18next';
import {useSelector, useDispatch} from 'react-redux';
import {Button, Form, Modal} from 'semantic-ui-react';
import FormField from '../../components/BaseComponents';
import {TEXT_TYPE} from "../../constants/columnTypes";
import {getProfileSettingsRequest, profileSettingsSelector} from "../../ducks/profile";
import PasswordField from "../../components/BaseComponents/Password";

const Profile = ({children}) => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    let [modalOpen, setModalOpen] = useState(false);
    let [form, setForm] = useState({});

    const profile = useSelector(state => profileSettingsSelector(state));

    useEffect(() => {
        setForm({...profile});
    }, [profile])

    const progress = false;

    const onOpen = () => {
        dispatch(getProfileSettingsRequest());
        setModalOpen(true);
    };

    const onClose = () => {
        setModalOpen(false);
    };

    const handleChange = (e, {name, value}) => {
        setForm(prevState => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleSave = () => {
    };

    console.log('form', form);

    return (
        <Modal
            dimmer="blurring"
            trigger={children}
            open={modalOpen}
            closeOnDimmerClick={false}
            onOpen={onOpen}
            onClose={onClose}
            closeIcon
            size="mini"
        >
            <Modal.Header>{'Настройки профиля'}</Modal.Header>
            <Modal.Content>
                <Modal.Description>
                    <Form>
                        <FormField name="userName" type={TEXT_TYPE} value={form['userName']} onChange={handleChange}/>
                        <FormField name="email" type={TEXT_TYPE} value={form['email']} onChange={handleChange}/>
                        <FormField name="role" type={TEXT_TYPE} value={form['roleName']} isReadOnly
                                   onChange={handleChange}/>
                        <FormField name="oldPassword" type={TEXT_TYPE} value={form['old_password']}
                                   onChange={handleChange}/>
                        <PasswordField/>
                    </Form>
                </Modal.Description>
            </Modal.Content>
            <Modal.Actions>
                <Button color="grey" onClick={onClose}>
                    {t('CancelButton')}
                </Button>
                <Button color="blue" loading={progress} onClick={handleSave}>
                    {t('SaveButton')}
                </Button>
            </Modal.Actions>
        </Modal>
    );
};

export default Profile;
