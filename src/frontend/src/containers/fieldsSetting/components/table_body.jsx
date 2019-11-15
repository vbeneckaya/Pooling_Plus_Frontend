import React from 'react';
import { Table } from 'semantic-ui-react';
import FieldCell from './field_cell';
import SettingCell from './setting_cell';
import CellValue from '../../../components/ColumnsValue';
import { ACTIVE_TYPE } from '../../../constants/columnTypes';

const TableBody = ({
    column,
    statusList,
    changeSettings,
    isExt,
    editProgress,
    t,
    toggleHidden,
}) => {
    return (
        <Table.Row key={column.fieldName}>
            <Table.Cell className="table-fields-setting_name">
                <FieldCell
                    field={column.fieldName}
                    isDisabled={column.isReadOnly || column.isHidden}
                    isExt={isExt}
                    t={t}
                    changeSettings={changeSettings}
                />
            </Table.Cell>
            <Table.Cell>
                <CellValue
                    value={column.isHidden}
                    type={ACTIVE_TYPE}
                    isDisabled={column.isReadOnly}
                    toggleIsActive={() => toggleHidden(column.fieldName, isExt)}
                />
            </Table.Cell>
            {statusList.map(status => (
                <Table.Cell key={`${status.name}_${column.fieldName}`}>
                    <SettingCell
                        value={column.accessTypes[status.name]}
                        isDisabled={column.isReadOnly || column.isHidden}
                        loading={
                            editProgress &&
                            (editProgress.field === column.fieldName &&
                                (!editProgress.state || editProgress.state === status.name))
                        }
                        onChange={(e, { value }) =>
                            changeSettings(column.fieldName, value, status.name, isExt)
                        }
                        t={t}
                    />
                </Table.Cell>
            ))}
        </Table.Row>
    );
};

export default React.memo(TableBody);
