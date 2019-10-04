import React from 'react';
import { Button, Checkbox, Dimmer, Form, Icon, Input, Loader, Popup } from 'semantic-ui-react';

import { connect } from 'react-redux';
import { withTranslation } from 'react-i18next';
import {clearLookup, getLookupRequest, listSelector, progressSelector} from "../../ducks/lookup";

class Facet extends React.Component {
    state = {};

    setFilter = (e, { value }) => {
        this.setState({ filter: value.toLowerCase() });
    };

    clearFilter = () => {
        this.setState({ filter: '' });
        this.props.clearLookup();
    };

    handleOpen = () => {
        const {source, getList} = this.props;

        getList({
            name: source,
            params: {}
        });
    };

    render() {
        const {
            name,
            text,
            value,
            onChange,
            sort,
            setSort,
            valuesList = [],
            loading,
            t,
        } = this.props;

        let values = value ? value.split('|') : [];

        let items = valuesList.filter(item => item.name) || [];

        items = items.map(x => {
            return {
                value: x.value,
                name: t(x.name),
                isActive: x.isActive,
            };
        });

        if (this.state.filter)
            items = items.filter(
                x => values.includes(x.value) || x.name.toLowerCase().includes(this.state.filter),
            );

        const toggle = (e, { value }) => {
            if (values.some(x => x === value)) {
                values.splice(values.indexOf(value), 1);
            } else {
                values.push(value);
            }
            if (onChange !== undefined) onChange(e, { name: name, value: values.join('|') });
        };

        let content = (
            <Form>
                <label className="label-in-popup">{t(name)}</label>
                <div>
                    <Input
                        fluid
                        size="mini"
                        icon="search"
                        value={this.state.filter}
                        onChange={this.setFilter}
                    />
                </div>
                <div className="select-facet-values">
                    <Dimmer active={loading} inverted>
                        <Loader size="small">Loading</Loader>
                    </Dimmer>
                    {items &&
                        items.map(x => {
                            let label = <label>{x.name}</label>;
                            return (
                                <Form.Field
                                    key={x.value}
                                    className={!x.isActive ? 'colorGrey' : ''}
                                >
                                    <Checkbox
                                        value={x.value}
                                        checked={values.includes(x.value)}
                                        onChange={toggle}
                                        label={label}
                                    />
                                </Form.Field>
                            );
                        })}
                </div>
            </Form>
        );

        return (
            <div className="facet-input">
                <Popup
                    trigger={
                        <Input
                            fluid
                            label={{ basic: true, content: '' }}
                            labelPosition="right"
                            onKeyPress={e => {
                                e.preventDefault();
                            }}
                            placeholder={
                                values.length > 0 ? values.length + ' ' + 'выбрано' : t(name)
                            }
                        />
                    }
                    content={content}
                    on="click"
                    hoverable
                    className="from-popup"
                    position="bottom left"
                    onOpen={this.handleOpen}
                    onClose={this.clearFilter}
                />
                <Button
                    className={`sort-button sort-button-up ${
                        sort === 'asc' ? 'sort-button-active' : ''
                    }`}
                    name={name}
                    value="asc"
                    onClick={setSort}
                >
                    <Icon name="caret up" />
                </Button>
                <Button
                    className={`sort-button sort-button-down ${
                        sort === 'desc' ? 'sort-button-active' : ''
                    }`}
                    name={name}
                    value="desc"
                    onClick={setSort}
                >
                    <Icon name="caret down" />
                </Button>
            </div>
        );
    }
}

const mapStateToProps = state => {
    return {
        valuesList: listSelector(state),
        loading: progressSelector(state)
    };
};

const mapDispatchToProps = dispatch => {
    return {
        getList: params => {
            dispatch(getLookupRequest(params))
        },
        clearLookup: () => {
            dispatch(clearLookup())
        }
    }
};

export default withTranslation()(connect(mapStateToProps, mapDispatchToProps)(Facet));
