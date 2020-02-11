import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector, useDispatch } from 'react-redux';
import { Button, Form, Modal, Segment, Tab, Label } from 'semantic-ui-react';
import FormField from '../../components/BaseComponents';
import { PASSWORD_TYPE, TEXT_TYPE } from '../../constants/columnTypes';
import {
    addError,
    changePasswordRequest,
    editProfileSettingsRequest,
    errorSelector,
    getProfileSettingsRequest,
    profileSettingsSelector,
    progressChangePasswordSelector,
    progressEditSelector,
} from '../../ducks/profile';
import PasswordField from '../../components/BaseComponents/Password';

const Profile = ({ children, open: modalOpen, onOpen, onClose }) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    // let [modalOpen, setModalOpen] = useState(false);
    let [form, setForm] = useState({});
    let [passwordForm, setPassword] = useState({});

    const profile = useSelector(state => profileSettingsSelector(state));

    useEffect(
        () => {
            setForm({ ...profile });
        },
        [profile],
    );

    const progressEdit = useSelector(state => progressEditSelector(state));

    const progressChangePassword = useSelector(state => progressChangePasswordSelector(state));

    const error = useSelector(state => errorSelector(state));

    const handleChange = (e, { name, value }) => {
        setForm(prevState => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleChangePassword = (e, { name, value }) => {
        setPassword(prevState => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleSave = () => {
        dispatch(
            editProfileSettingsRequest({
                form,
                callbackSuccess: handleClose,
            }),
        );
    };

    const handleComparePassword = () => {
        const { returnNewPassword, newPassword } = passwordForm;

        if (returnNewPassword !== newPassword) {
            dispatch(
                addError({
                    name: 'returnNewPassword',
                    message: t('passwords_do_not_match'),
                }),
            );
            return false;
        }

        return true;
    };

    const handleSaveNewPassword = () => {
        if (handleComparePassword()) {
            dispatch(
                changePasswordRequest({
                    form: passwordForm,
                    t,
                    callbackSuccess: () => {
                        setPassword({});
                    },
                }),
            );
        }
    };

    const handleClose = () => {
        setPassword({});
        onClose();
    };

    return (
        <Modal
            dimmer="blurring"
            open={modalOpen}
            closeOnDimmerClick={false}
            onOpen={onOpen}
            onClose={handleClose}
            closeIcon
            size="tiny"
        >
            <Modal.Header>{t('profile_settings')}</Modal.Header>
            <Modal.Content>
                <Modal.Description>
                    <Form>
						<Tab panes={[
						  { menuItem: 'Информация', render: () => <Tab.Pane>
							<FormField
								name="userName"
								type={TEXT_TYPE}
								value={form['userName']}
								isRequired
								error={error['userName']}
								onChange={handleChange}
							/>
							<FormField
								name="email"
								type={TEXT_TYPE}
								value={form['email']}
								isRequired
								error={error['email']}
								onChange={handleChange}
							/>
							<FormField
								name="role"
								type={TEXT_TYPE}
								value={form['roleName']}
								isReadOnly
								onChange={handleChange}
							/>
							<Segment>
								<FormField
									name="oldPassword"
									type={PASSWORD_TYPE}
									value={passwordForm['oldPassword']}
									error={error['oldPassword']}
									onChange={handleChangePassword}
								/>
								<FormField
									name="newPassword"
									type={PASSWORD_TYPE}
									value={passwordForm['newPassword']}
									onChange={handleChangePassword}
								/>
								<FormField
									name="returnNewPassword"
									type={PASSWORD_TYPE}
									value={passwordForm['returnNewPassword']}
									error={error['returnNewPassword']}
									onChange={handleChangePassword}
									onBlur={handleComparePassword}
								/>
								<div className="change_password">
									<Button
										loading={progressChangePassword}
										onClick={handleSaveNewPassword}
									>
										{t('set_new_password')}
									</Button>
								</div>
							</Segment>						  
						  
						  
						  </Tab.Pane> },
						  { menuItem: 'Доступ к личным кабинетам', render: () => <Tab.Pane>
                                  
                            <Segment>
                                <Label attached='top'>pooling.me</Label>
                                <FormField
                                    name="poolingLogin"
                                    type={TEXT_TYPE}
                                    value={form['poolingLogin']}
                                    isRequired
                                    error={error['poolingLogin']}
                                    onChange={handleChange}
                                />						  
                                <FormField
                                    name="poolingPassword"
                                    type={PASSWORD_TYPE}
                                    value={form['poolingPassword']}
                                    isRequired
                                    error={error['poolingPassword']}
                                    onChange={handleChange}
                                />
                            </Segment>
                            <Segment>
                                <Label attached='top'>lk.fmlogistic.com</Label>
                                <FormField
                                    name="fmCPLogin"
                                    type={TEXT_TYPE}
                                    value={form['fmCPLogin']}
                                    isRequired
                                    error={error['fmCPLogin']}
                                    onChange={handleChange}
                                />						  
                                <FormField
                                    name="fmCPPassword"
                                    type={PASSWORD_TYPE}
                                    value={form['fmCPPassword']}
                                    isRequired
                                    error={error['fmCPPassword']}
                                    onChange={handleChange}
                                />
                            </Segment>
						  </Tab.Pane> }
						]
						} />
                    </Form>
                </Modal.Description>
            </Modal.Content>
            <Modal.Actions>
                <Button color="grey" onClick={handleClose}>
                    {t('CancelButton')}
                </Button>
                <Button color="blue" loading={progressEdit} onClick={handleSave}>
                    {t('SaveButton')}
                </Button>
            </Modal.Actions>
        </Modal>
    );
};

export default Profile;
