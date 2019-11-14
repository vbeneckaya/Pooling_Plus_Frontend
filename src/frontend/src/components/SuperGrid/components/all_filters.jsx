import React, {useState, useEffect} from 'react';
import {Button, Grid, Icon} from "semantic-ui-react";
import {useSelector} from "react-redux";
import {useTranslation} from "react-i18next";
import {representationFromGridSelector} from "../../../ducks/representations";

const AllFilters = ({ gridName, filter }) => {
    const columns = useSelector(state => representationFromGridSelector(state, gridName));
    const { t } = useTranslation();


    return (
        <div className="all-filters-popup">
            <div className="all-filters-header">
            <h3>Filter columns</h3>
                <Button size="mini">Clear filter</Button>
            </div>
            <Grid style={{paddingBottom: "12px"}}>
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <h4>Column</h4>
                    </Grid.Column>
                    <Grid.Column>
                        <h4>Value</h4>
                    </Grid.Column>
                </Grid.Row>
                {
                    Object.keys(filter).map(key => (
                        <Grid.Row columns={2}>
                            <Grid.Column>{t(key)}</Grid.Column>
                            <Grid.Column>{filter[key]}</Grid.Column>
                        </Grid.Row>
                    ))
                }
            </Grid>
            <Button size="mini">
                <Icon name="add"/>
                Add filter
            </Button>
        </div>
    )
};

export default AllFilters;
