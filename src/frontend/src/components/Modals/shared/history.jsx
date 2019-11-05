import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import {
    clearHistory,
    getHistoryRequest,
    historySelector,
    progressSelector,
} from '../../../ducks/history';
import { Dimmer, Grid, Loader } from 'semantic-ui-react';
import { dateToUTC } from '../../../utils/dateTimeFormater';

const History = ({ cardId, status }) => {
    const dispatch = useDispatch();
    const history = useSelector(state => historySelector(state));

    useEffect(
        () => {
            dispatch(getHistoryRequest(cardId));
            return () => {
                dispatch(clearHistory());
            };
        },
        [status],
    );

    const loading = useSelector(state => progressSelector(state));

    return (
        <div>
            <Grid>
                <Dimmer active={loading} inverted>
                    <Loader size="huge">Loading</Loader>
                </Dimmer>
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
