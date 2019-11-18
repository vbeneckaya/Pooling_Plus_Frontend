import React from 'react';
import { Button, Popup } from 'semantic-ui-react';

const FieldCell = ({ field, t, changeSettings, isExt, isDisabled }) => {
    /* console.log('fieldCell');*/
    return (
        <>
            <b>{t(field)}</b>
            <Popup
                trigger={<Button size="mini" disabled={isDisabled} className="margin-left-8">{t('All')}</Button>}
                content={
                    <Button.Group>
                        {/*<Button
                            size="mini"
                            onClick={() => {
                                changeSettings(field, 'hidden', null, isExt);
                            }}
                        >
                            {t('hidden')}
                        </Button>*/}
                        <Button
                            size="mini"
                            onClick={() => {
                                changeSettings(field, 'show', null, isExt);
                            }}
                        >
                            {t('show')}
                        </Button>
                        <Button
                            size="mini"
                            onClick={() => {
                                changeSettings(field, 'edit', null, isExt);
                            }}
                        >
                            {t('edit')}
                        </Button>
                    </Button.Group>
                }
                on="click"
                position="top left"
            />
        </>
    );
};

export default React.memo(FieldCell, (prevProps, nextProps) => {
    return prevProps.field === nextProps.field && nextProps.isDisabled === prevProps.isDisabled;
});
