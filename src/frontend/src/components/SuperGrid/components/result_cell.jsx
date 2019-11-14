import React, { useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import CellValue from '../../ColumnsValue';
import { Button, Form, Icon, Loader, Modal, Table } from 'semantic-ui-react';
import { toast } from 'react-toastify';
import {
    checkForEditingRequest,
    checkProgressSelector,
    editModalSelector,
} from '../../../ducks/gridColumnEdit';

import FormField from '../../BaseComponents';
import { LINK_TYPE } from '../../../constants/columnTypes';
import { ORDERS_GRID } from '../../../constants/grids';
import { invokeMassUpdateRequest, progressMassUpdateSelector } from '../../../ducks/gridActions';

const ModalComponent = ({ element, props, children }) => {
    if (!element) {
        return <>{children}</>;
    }
    return React.cloneElement(element, props, children);
};

const CellResult = ({row, column, loadList, indexRow, indexColumn, modalCard, gridName, t}) => {
    const dispatch = useDispatch();
    const checkProgress = useSelector(state => checkProgressSelector(state));
    const editProgress = useSelector(state => progressMassUpdateSelector(state));
    const editModal = useSelector(state => editModalSelector(state));
    const contextRef = useRef(null);

    let [open, setOpen] = useState(false);
    let [value, setValue] = useState(null);

    const copyToClipboard = () => {
        const text = contextRef.current && contextRef.current.textContent;
        navigator.clipboard &&
            navigator.clipboard.writeText(text).then(
                () => {
                    toast.info(t('copied_to_clipboard_success'));
                },
                error => {
                    toast.error(t('copied_to_clipboard_error', { error }));
                },
            );
    };

    const handleClick = (rowId, fieldName, state) => {
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

    console.log('cell');

    return (
        <>
            <Table.Cell
                className="value-cell"
            >
                <div className="cell-grid">
                    <div
                        className={`cell-grid-value ${
                            row[column.name] !== null ? '' : 'cell-grid-value_empty'
                        }`}
                        ref={contextRef}
                        onClick={e =>
                            column.type !== LINK_TYPE
                                ? handleClick(row.id, column.name, row.status)
                                : e.stopPropagation()
                        }
                    >
                        <CellValue
                            {...column}
                            indexRow={indexRow}
                            indexColumn={indexColumn}
                            value={row[column.name]}
                            width={column.width}
                            t={t}
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
                            <Loader active={true} size="mini" />
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
                                onChange={(e, { value }) => {
                                    setValue(value);
                                }}
                            />
                        </Form>
                    </Modal.Description>
                </Modal.Content>
                <Modal.Actions>
                    <Button onClick={handleClose}>{t('cancelConfirm')}</Button>
                    <Button
                        color="primary"
                        disabled={value === row[column.name]}
                        onClick={handleSave}
                    >
                        {t('SaveButton')}
                    </Button>
                </Modal.Actions>
            </Modal>
        </>
    );
};

export default React.memo(CellResult, (prevProps = {}, nextProps = {}) => {
    return prevProps.value === nextProps.value && prevProps.column.width === nextProps.column.width;
});
