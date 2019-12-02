import React, {useRef, useState, useCallback} from 'react';
import CellValue from '../../ColumnsValue';
import { Button, Form, Icon, Loader, Modal, Table } from 'semantic-ui-react';
import { toast } from 'react-toastify';
import FormField from '../../BaseComponents';
import { LINK_TYPE } from '../../../constants/columnTypes';
import { ORDERS_GRID } from '../../../constants/grids';

const ModalComponent = ({ element, props, children }) => {
    if (!element) {
        return <>{children}</>;
    }
    return React.cloneElement(element, props, children);
};

const BodyCell = ({
                      column,
                      loadList,
                      indexRow,
                      indexColumn,
                      modalCard,
                      gridName,
                      t,
                      checkForEditing,
                      invokeMassUpdate,
                      ...row
                  }) => {
    const contextRef = useRef(null);

    let [open, setOpen] = useState(false);
    let [value, setValue] = useState(null);
    let [progress, setProgress] = useState(false);

    const copyToClipboard = () => {
        const text = contextRef.current && contextRef.current.textContent;
        navigator.clipboard &&
        navigator.clipboard.writeText(text).then(
            () => {
                toast.info(t('copied_to_clipboard_success'));
            },
            error => {
                toast.error(t('copied_to_clipboard_error', {error}));
            },
        );
    };

    const handleClick = (rowId, fieldName, state) => {
        setProgress(true);
        checkForEditing({
            rowId,
            fieldName,
            state,
            forEntity: gridName,
            t,
            callbackSuccess: () => {
                handleOpen();
            },
            callbackFunc: () => {
                setProgress(false);
            },
        });
    };

    const handleOpen = () => {
        setOpen(true);
        setValue(row[column.name]);
    };

    const handleClose = () => {
        setOpen(false);
        setValue(null);
    };

    const handleSave = () => {
        setProgress(true);
        handleClose();
        invokeMassUpdate({
            ids: [row.id],
            name: gridName,
            field: column.name,
            value,
            callbackSuccess: () => {
                loadList(false, true);
            },
            callbackFunc: () => {
                setProgress(false);
            },
        });
    };

    const handleChange = useCallback((e, {value}) => {
        setValue(value);
    }, []);

    const handleCellClick = useCallback(e => {
        column.type !== LINK_TYPE
            ? handleClick(row.id, column.name, row.status)
            : e.stopPropagation()
    }, [column.type, row.id, column.name, row.status]);

    console.log('BodyCell');

    return (
        <>
            <Table.Cell className="value-cell">
                <div className="cell-grid">
                    <div
                        className={`cell-grid-value ${
                            row[column.name] !== null ? '' : 'cell-grid-value_empty'
                            }`}
                        ref={contextRef}
                        onClick={handleCellClick}
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
                                    element={modalCard()}
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
                        {progress ? (
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
                                {...column}
                                value={value}
                                onChange={handleChange}
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

export default React.memo(BodyCell, (prevProps = {}, nextProps = {}) => {
    return prevProps.value === nextProps.value && prevProps.column.width === nextProps.column.width;
});
