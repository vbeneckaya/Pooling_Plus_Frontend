import React, {useRef} from 'react';
import {useTranslation} from 'react-i18next';
import {Button, Icon, Popup} from 'semantic-ui-react';
import './style.scss';
import {
    BIG_TEXT_TYPE,
    BOOLEAN_TYPE,
    DATE_TIME_TYPE,
    DATE_TYPE,
    ENUM_TYPE,
    LINK_TYPE,
    NUMBER_TYPE,
    SELECT_TYPE,
    STATE_TYPE,
    TEXT_TYPE,
    TIME_TYPE,
    LOCAL_DATE_TIME
} from '../../constants/columnTypes';
import TextFacet from './Text';
import NumberFacet from './Number';
import SelectFacet from './Select';
import DateFacet from './Date';
import TimeFacet from './Time';
import StateFacet from './State';
import Bool from './Bool';

const getTypeFacet = {
    [TEXT_TYPE]: <TextFacet />,
    [BIG_TEXT_TYPE]: <TextFacet />,
    [NUMBER_TYPE]: <NumberFacet />,
    [SELECT_TYPE]: <SelectFacet />,
    [DATE_TIME_TYPE]: <DateFacet />,
    [DATE_TYPE]: <DateFacet />,
    [LOCAL_DATE_TIME]: <DateFacet/>,
    [TIME_TYPE]: <TimeFacet/>,
    [STATE_TYPE]: <StateFacet />,
    [BOOLEAN_TYPE]: <Bool />,
    [ENUM_TYPE]: <SelectFacet />,
    [LINK_TYPE]: <TextFacet />,
};

const Control = props => {
    const { type } = props;

    return React.cloneElement(getTypeFacet[type], props);
};

const FacetField = ({name, sort: sortObj, setSort, type, value, setFilter, source, index}) => {
    const { t } = useTranslation();
    let sort = null;

    if (sortObj && sortObj.name === name) {
        sort = sortObj.desc ? 'desc' : 'asc';
    }

    const handleSort = () => {
        if (sort === 'desc') {
            setSort({
                name,
                desc: false,
            });
        } else if (sort === 'asc') {
            setSort({});
        } else {
            setSort({
                name,
                desc: true,
            });
        }
    };

    const contextRef = useRef(null);

    return (
        <div className="facet">
            <div className="facet-field" onClick={handleSort} ref={contextRef}>
                {t(name)}
            </div>
            <div className="facet-actions">
                <div
                    className={
                        value ? 'facet-actions__filter_active' : 'facet-actions__filter'
                    }
                >
                    <Popup
                        trigger={
                            <Button>
                                <Icon name="filter" />
                            </Button>
                        }
                        context={contextRef}
                        content={
                            <Control
                                type={type}
                                name={name}
                                value={value}
                                source={source}
                                onChange={setFilter}
                            />
                        }
                        pinned
                        position={index === 0 ? "bottom left" : "bottom right"}
                        className="from-popup"
                        on="click"
                    />
                </div>

                <div className="facet-actions__sort">
                    {sort && sort === 'asc' ? <Icon name="sort amount up" /> : null}
                    {sort && sort === 'desc' ? <Icon name="sort amount down" /> : null}
                </div>
            </div>
        </div>
    );
};

export default React.memo(FacetField, (prevProps, nextProps) => {
    return prevProps.value === nextProps.value && prevProps.sort.name === nextProps.sort.name && prevProps.sort.desc === nextProps.sort.desc;
});
