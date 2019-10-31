import React from 'react';
import CellValue from '../../ColumnsValue';
import {Icon, Table} from 'semantic-ui-react';
import {toast} from "react-toastify";

const ModalComponent = ({element, props, children}) => {
    if (!element) {
        return <>{children}</>;
    }
    return React.cloneElement(element, props, children);
};

const CellResult = ({row, column, loadList, indexRow, modalCard, gridName}) => {
    const copyToClipboard = text => {
        navigator.clipboard && navigator.clipboard.writeText(text).then(
            () => {
                toast.info('Скопировано в буфер обмена');
            },
            (err) => {
                toast.error(`При копировании произошла ошибка: ${err}`);
            },
        );
    };

    return (
        <Table.Cell className={column.name.toLowerCase().includes('address') ? 'address-cell' : ''}>
            {
                <div className="cell-grid">
                    <div
                        className={`cell-grid-value ${
                            row[column.name] !== null ? '' : 'cell-grid-value_empty'
                            }`}
                    >
                        <CellValue
                            {...column}
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
                    {row[column.name] !== null ? (
                        <div className="cell-grid-copy-btn">
                            <Icon
                                name="clone outline"
                                size="small"
                                onClick={() => copyToClipboard(row[column.name])}
                            />
                        </div>
                    ) : null}
                </div>
            }
        </Table.Cell>
    );
};

export default CellResult;
