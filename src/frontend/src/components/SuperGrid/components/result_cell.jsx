import React, {useRef, useState} from 'react';
import {useDispatch, useSelector} from 'react-redux';
import {useTranslation} from 'react-i18next';
import CellValue from '../../ColumnsValue';
import {Button, Form, Icon, Loader, Modal, Popup, Table} from 'semantic-ui-react';
import {toast} from 'react-toastify';
import {
    checkForEditingRequest,
    checkProgressSelector,
    editModalSelector,
} from '../../../ducks/gridColumnEdit';

import FormField from '../../BaseComponents';
import {LINK_TYPE} from '../../../constants/columnTypes';
import {ORDERS_GRID} from '../../../constants/grids';
import {invokeMassUpdateRequest, progressMassUpdateSelector} from '../../../ducks/gridActions';

const ModalComponent = ({element, props, children}) => {
    if (!element) {
        return <>{children}</>;
    }
    return React.cloneElement(element, props, children);
};

const CellResult = ({row, column, loadList, indexRow, indexColumn, modalCard, gridName}) => {
    const dispatch = useDispatch();
    const {t} = useTranslation();
    const checkProgress = useSelector(state => checkProgressSelector(state));
    const editProgress = useSelector(state => progressMassUpdateSelector(state));
    const editModal = useSelector(state => editModalSelector(state));
    const contextRef = useRef(null);

    let [open, setOpen] = useState(false);
    let [value, setValue] = useState(null);

    const copyToClipboard = () => {
        console.log('contextRef', contextRef);
        const text = contextRef.current && contextRef.current.textContent;
        navigator.clipboard &&
        navigator.clipboard.writeText(text).then(
            () => {
                toast.info('Скопировано в буфер обмена');
            },
            err => {
                toast.error(`При копировании произошла ошибка: ${err}`);
            },
        );
    };

    const handleClick = (rowId, fieldName, state) => {
        console.log('click');
        dispatch(
            checkForEditingRequest({
                rowId,
                fieldName,
                state,
                forEntity: gridName,
                t,
                callbackSuccess: () => {
                    handleOpen();
                },
            }),
        );
    };

    const handleOpen = () => {
        setValue(row[column.name]);
        setOpen(true);
    };

    const handleClose = () => {
        setValue(null);
        setOpen(false);
    };

    const handleSave = () => {
        handleClose();
        dispatch(
            invokeMassUpdateRequest({
                ids: [row.id],
                name: gridName,
                field: column.name,
                value,
                callbackSuccess: () => {
                    loadList(false, true);
                },
            }),
        );
    };

    return (
        <>
            <Table.Cell
                className={column.name.toLowerCase().includes('address') ? 'address-cell' : ''}
            >
                <div className="cell-grid">
                    <div
                        className={`cell-grid-value ${
                            row[column.name] !== null ? '' : 'cell-grid-value_empty'
                            }`}
                        onClick={e =>
                            column.type !== LINK_TYPE
                                ? handleClick(row.id, column.name, row.status)
                                : e.stopPropagation()
                        }
                    >
                        <CellValue
                            {...column}
                            ref={contextRef}
                            id={`${row.id}_${column.name}_${indexRow}`}
                            indexRow={indexRow}
                            value={row[column.name]}
                            modalCard={
                                <ModalComponent
                                    element={modalCard}
                                    props={{
                                        ...row,
                                        loadList,
                                        title: `edit_${gridName}`,
                                    }}
                                    key={`modal_${row.id}`}
                                />
                            }
                        />
                    </div>
                    <div>
                        {(checkProgress &&
                            row.id === checkProgress.rowId &&
                            column.name === checkProgress.fieldName) ||
                        (editProgress &&
                            row.id === editModal.rowId &&
                            column.name === editModal.fieldName) ? (
                            <Loader active={true} size="mini"/>
                        ) : (
                            <>
                                {row[column.name] !== null ? (
                                    <div className="cell-grid-copy-btn">
                                        <Icon
                                            name="clone outline"
                                            size="small"
                                            onClick={() => copyToClipboard(row[column.name])}
                                        />
                                    </div>
                                ) : null}
                            </>
                        )}
                    </div>
                </div>
            </Table.Cell>
            <Modal open={open} size="tiny" closeOnDimmerClick={false}>
                <Modal.Header>
                    {t(`edit_${gridName}`, {
                        number: gridName === ORDERS_GRID ? row.orderNumber : row.shippingNumber,
                        status: t(row.status),
                    })}
                </Modal.Header>
                <Modal.Content>
                    <Modal.Description>
                        <Form>
                            <FormField
                                column={column}
                                value={value}
                                onChange={(e, {value}) => {
                                    setValue(value);
                                }}
                            />
                        </Form>
                    </Modal.Description>
                </Modal.Content>
                <Modal.Actions>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button color="primary" onClick={handleSave}>
                        Ok
                    </Button>
                </Modal.Actions>
            </Modal>
        </>
    );
};

export default CellResult;
