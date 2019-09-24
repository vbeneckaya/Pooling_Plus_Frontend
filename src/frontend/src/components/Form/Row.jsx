import React from 'react';
import {Grid} from 'semantic-ui-react';
import FormField from "../BaseComponents";


const RowForm = ({fields, form, onChange}) => {
    console.log();
    return (
        <Grid.Row columns={fields.length}>
            {
                fields.map(field => <Grid.Column>
                    <FormField
                        column={field}
                        value={form[field.name]}
                        onChange={onChange}
                    />
                </Grid.Column>)
            }
        </Grid.Row>);
};

export default RowForm;
