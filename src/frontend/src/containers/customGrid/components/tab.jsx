import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import { tabViewSelector } from '../../../ducks/gridCard';
import { ROW_TAB_TYPE } from '../../../constants/formTypes';
import RowForm from '../../../components/Form/Row';
import {Form, Grid} from "semantic-ui-react";

const getComponent = {
    [ROW_TAB_TYPE]: <RowForm />,
};


const TabCard = ({ name }) => {
    const views = useSelector(state => tabViewSelector(state, name));

    let [form, setForm] = useState({});

    const onChange = (event, {name, value}) => {
        console.log('onChange', name, value);
        setForm({
            ...form,
            [name]: value
        })
    };

    return (
        <Form>
            <Grid>
                {views.map(view => React.cloneElement(getComponent[view.type], {...view, form, onChange}))}
            </Grid>
        </Form>
    );
};

export default TabCard;
