import React, {Component} from 'react';
import {withTranslation} from 'react-i18next';
import _ from 'lodash';
import { debounce } from 'throttle-debounce';

import './style.scss';
import Filter from './components/filter';
import InfiniteScrollTable from '../InfiniteScrollTable';

import Result from './components/result';
import {Confirm, Loader} from 'semantic-ui-react';
import {withRouter} from 'react-router-dom';

const initState = (columns) => ({
    columns: columns,
});

class MiniOrdersGrid extends Component {
    constructor(props) {
        super(props);
        const {columns} = this.props;
        this.state = {
            ...initState(columns),
        };

    }

    pageLoading = () => {
        const {columns} = this.state;
        const width = this.container.offsetWidth - 65;
        this.setState(
            {
                columns: columns.map(item => ({
                    ...item,
                    width: item.width || parseInt(width / columns.length),
                })),
            },
        );
    };

    componentDidMount() {
        this.pageLoading();
    }

    // setFilterApiAndLoadList = () => {
    //     // const {editRepresentation, representationName, name} = this.props;
    //     // const {columns} = this.state;
    //     //
    //     // editRepresentation({
    //     //     key: name,
    //     //     name: representationName,
    //     //     oldName: representationName,
    //     //     value: columns,
    //     // });
    //     this.loadAndResetContainerScroll();
    // };

    //debounceSetFilterApiAndLoadList = debounce(300, this.setFilterApiAndLoadList);

    // loadAndResetContainerScroll = () => {
    //    
    //      this.loadList();
    //     if (this.container && this.container.scrollTop) {
    //         this.container.scrollTop = 0;
    //     }
    // };

    resizeColumn = (size, index) => {
        const {columns} = this.state;
        clearTimeout(this.timer);
        this.setState(prevState => {
            const nextColumns = [...prevState.columns];
            nextColumns[index] = {
                ...nextColumns[index],
                width: size.width,
            };
            return {
                columns: nextColumns,
            };
        });

        let sum = 0;
        columns.forEach(item => {
            sum = sum + item.width + columns.length + 50;
        });

        this.timer = setTimeout(() => {
            this.pageLoading();
        }, 2000);
    };

    handleGoToCard = (isEdit, id, name) => {
        const {history, cardLink, newLink} = this.props;

        history.push({
            pathname: isEdit
                ? cardLink.replace(':name', name).replace(':id', id)
                : newLink.replace(':name', name),
            state: {
                pathname: history.location.pathname,
            },
        });
    };

    render() {
        const {
            rows,
            columns,
            openOrderModal,
            isEditBtn,
            isDeleteBtn,
            removeFromShipping,
            //    actions,
            //    isShowActions,
            confirmation = {},
            closeConfirmation = () => {
            },
            name,
            t,
        } = this.props;

        return (
            <>
                <div
                    className={`scroll-grid-container grid_small`}
                    ref={instance => {this.container = instance;}}
                >
                    <InfiniteScrollTable
                        className="grid-table"
                        unstackable
                        celled={false}
                        selectable={false}
                        columns={this.state && this.state.columns}
                        fixed
                        headerRow={
                            <Filter
                                columns={this.state && this.state.columns}
                                showCheckBoxField={false}
                                showIcons={false}
                                isShowActions={isDeleteBtn || isDeleteBtn}
                                gridName={name}
                                resizeColumn={this.resizeColumn}
                            />
                        }
                        context={this.container}
                    >
                        <Result
                            columns={columns}
                            rows={rows}
                            progress={false}
                            name={name}
                            gridName={name}
                            openOrderModal={openOrderModal}
                            //goToCard={this.handleGoToCard}
                            //    actions={actions}
                            isShowEditButton={isEditBtn}
                            isShowDeleteButton={isDeleteBtn}
                            removeFromShipping={removeFromShipping}
                        />
                    </InfiniteScrollTable>
                </div>
                <Confirm
                    dimmer="blurring"
                    open={confirmation.open}
                    onCancel={closeConfirmation}
                    onConfirm={confirmation.onConfirm}
                    cancelButton={t('cancelConfirm')}
                    content={confirmation.content}
                />
            </>
        );
    }
}

export default withTranslation()(withRouter(MiniOrdersGrid));
