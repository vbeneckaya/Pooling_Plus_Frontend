import React, { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';

import { Icon } from 'semantic-ui-react';
import { getLookupRequest, valuesListSelector } from '../../ducks/lookup';

const StateValue = ({ value, source }) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();

    let stateColors = useSelector(state => valuesListSelector(state, source)) || [];

    if (!stateColors.length) {
        dispatch(
            getLookupRequest({
                name: source,
                isForm: true,
                isSearch: true,
            }),
        );
    }

    const state = stateColors.find(x => x.name === value);
    const color = state ? state.color : 'grey';

    return (
        <div>
            <Icon color={color.toLowerCase()} name="circle" />
            {t(value)}
        </div>
    );
};

export default StateValue;
