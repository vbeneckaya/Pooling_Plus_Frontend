import React, { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { getHistoryRequest, historySelector } from '../../../ducks/history';
import {Grid} from "semantic-ui-react";
import {dateToUTC, formatDate} from "../../../utils/dateTimeFormater";

const History = ({ cardId }) => {
    const dispatch = useDispatch();
    const history = useSelector(state => historySelector(state));

    useEffect(() => {
        dispatch(getHistoryRequest(cardId));
    }, []);

    console.log('history', history);

    return (
        <div>
            <Grid>
                {(history || []).map((historyItem, i) => (
                    <Grid.Row key={i}>
                        <Grid.Column width={5}>
                            <div>{dateToUTC(historyItem.createdAt, 'DD.MM.YYYY HH:mm')}</div>
                            <div className="history-who">{historyItem.userName}</div>
                        </Grid.Column>
                        <Grid.Column width={11}>
                            <div className="history-comment"> {historyItem.message} </div>
                        </Grid.Column>
                    </Grid.Row>
                ))}
            </Grid>
        </div>
    );
};

export default History;
