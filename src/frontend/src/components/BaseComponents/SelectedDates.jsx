import React from 'react';
import DatePicker from 'react-datepicker';
import { Form, Icon, Popup } from 'semantic-ui-react';

function SelectedDates({ dates = [] }) {
    const input = <Icon name="calendar alternate outline" size="large" />;

    const highlightWithRanges = [
        {
            selected_dates_color: dates ? dates.map(item => new Date(item)) : [],
        },
    ];

    const content = (
        <Form className="filter-popup">
            <Form.Group>
                <Form.Field>
                    <DatePicker
                        inline
                        locale="ru"
                        dateFormat="dd.MM.yyyy"
                        highlightDates={highlightWithRanges}
                        allowSameDay
                    />
                </Form.Field>
            </Form.Group>
        </Form>
    );

    return (
        <Popup
            trigger={input}
            content={content}
            on="click"
            hideOnScroll
            className="from-popup"
            position="bottom center"
        />
    );
}
export default SelectedDates;
