import React, { useState, useEffect } from 'react';
import { Input, Icon, Button } from 'semantic-ui-react';

const CellEdit = ({ value, onChange, name }) => {
    let [isEdit, setIsEdit] = useState(false);
    let [val, setVal] = useState(value);

    useEffect(() => {
        setVal(value);
    }, [value]);

    const handleChange = () => {
        setIsEdit(!isEdit);
        if (val !== value && isEdit) {
            onChange(val, name);
        }
    };

    return (
        <div className="cell-edit">
            <Input value={val} disabled={!isEdit} onChange={(e, { value }) => setVal(value)} />
            <Button icon>
                <Icon name={isEdit ? 'check' : 'edit'} onClick={handleChange} />
            </Button>
        </div>
    );
};

export default CellEdit;
