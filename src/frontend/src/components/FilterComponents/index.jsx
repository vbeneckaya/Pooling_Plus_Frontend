import React from 'react';
import {useTranslation} from 'react-i18next';
import {Button, Icon} from 'semantic-ui-react';
import './style.scss';

const FacetField = ({name, sort, setSort}) => {
    const {t} = useTranslation();

    return (
        <div className="facet">
            <div className="facet-field">
                {t(name)}
            </div>
            <div className="facet-actions">
                <div className="facet-actions__filter">
                    <Button>
                        <Icon name="filter"/>
                    </Button>
                </div>
                <div className="facet-actions__sort">
                    <Button
                        className={`sort-button sort-button-up ${
                            sort === 'asc' ? 'sort-button-active' : ''
                            }`}
                        name={name}
                        value="asc"
                        onClick={setSort}
                    >
                        <Icon name="caret up"/>
                    </Button>
                    <Button
                        className={`sort-button sort-button-down ${
                            sort === 'desc' ? 'sort-button-active' : ''
                            }`}
                        name={name}
                        value="desc"
                        onClick={setSort}
                    >
                        <Icon name="caret down"/>
                    </Button>
                </div>
            </div>
        </div>
    );
};

export default FacetField;
