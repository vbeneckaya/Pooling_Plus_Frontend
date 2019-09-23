import React from 'react';
import {DATE_TYPE, GROUP_TYPE, SELECT_TYPE, STATE_TYPE, TEXT_TYPE} from '../../constants/columnTypes';
import Text from '../BaseComponents/Text';
import {Grid} from 'semantic-ui-react';

const getComponent = {
    [TEXT_TYPE]: <Text />,
    [STATE_TYPE]: <Text/>,
    [DATE_TYPE]: <Text/>,
    [GROUP_TYPE]: <Text/>,
    [SELECT_TYPE]: <Text/>
};

const RowForm = ({fields}) => {
    console.log('fields', fields);
    return (
        <Grid.Row columns={fields.length}>
            {
                fields.map(field => <Grid.Column>{React.cloneElement(getComponent[field.type], field)}</Grid.Column>)
            }
        </Grid.Row>);
};

export default RowForm;
