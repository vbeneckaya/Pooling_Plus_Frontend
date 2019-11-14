import React from 'react';
import { useTranslation } from 'react-i18next';
import { DragDropContext, Draggable, Droppable } from 'react-beautiful-dnd';
import { Label } from 'semantic-ui-react';

const DragAndDropFields = ({ type, fieldsConfig, fieldsList, search, onChange }) => {
    /* let showed = (fieldsConfig.order || [])
         .filter(x => fieldsList.some(y => y.name === x))
         .concat(
             fieldsList
                 .filter(
                     x =>
                         !(fieldsConfig.hidden || []).includes(x.name) &&
                         !(fieldsConfig.order || []).includes(x.name),
                 )
                 .map(x => x.name),
         );*/

    const { t } = useTranslation();

    return (
        <div className="flex-container-justify">
            {(fieldsList && fieldsList.length > 0) || (fieldsConfig && fieldsConfig.length) ? (
                <DnDList
                    key={'dnd' + type}
                    type={type}
                    left={fieldsList}
                    right={fieldsConfig}
                    search={search}
                    t={t}
                    onChange={onChange}
                />
            ) : null}
        </div>
    );
};

// a little function to help us with reordering the result
const reorder = (list, startIndex, endIndex) => {
    const result = Array.from(list);
    const [removed] = result.splice(startIndex, 1);
    result.splice(endIndex, 0, removed);

    return result;
};

const move = (source, destination, droppableSource, droppableDestination, search) => {
    const sourceClone = Array.from(source);
    const destClone = Array.from(destination);
    const [removed] = sourceClone.splice(droppableSource.index, 1);

    destClone.splice(search ? destClone.length : droppableDestination.index, 0, removed);

    const result = {};
    result[droppableSource.droppableId] = sourceClone;
    result[droppableDestination.droppableId] = destClone;

    return result;
};

const getItemStyle = (isDragging, draggableStyle) => {
    const modalEl = document.getElementById('fieldModal');
    if (modalEl && /Chrome/.test(navigator.userAgent)) {
        let rect = modalEl.getBoundingClientRect();
        draggableStyle.top = draggableStyle.top - rect.top || 0;
        draggableStyle.left = draggableStyle.left - rect.left || 0;
    }
    return {
        userSelect: 'none',
        padding: 5,
        ...draggableStyle,
    };
};

const sortFunc = (item, t) => {
    item.sort(function(a, b) {
        const nameA = t(a.id).toLowerCase();
        const nameB = t(b.id).toLowerCase();
        if (nameA < nameB)
            //сортируем строки по возрастанию
            return -1;
        if (nameA > nameB) return 1;
        return 0; // Никакой сортировки
    });

    return item;
};

class DnDList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            items: sortFunc(this.props.left.map(x => ({ id: x.name, content: x })), props.t),
            selected: this.props.right.map(x => ({ id: x.name, content: x })),
        };
    }

    id2List = {
        droppable: 'items',
        droppable2: 'selected',
    };

    getList = id => this.state[this.id2List[id]];

    onDragEnd = result => {
        const { source, destination } = result;
        let state = {
            items: sortFunc(this.state.items, this.props.t),
            selected: this.state.selected,
        };
        // dropped outside the list
        if (!destination) {
            return;
        }

        if (source.droppableId === destination.droppableId) {
            const items = reorder(
                this.getList(source.droppableId),
                source.index,
                destination.index,
            );
            if (source.droppableId === 'droppable2') {
                state.selected = items;
            } else {
                state.items = sortFunc(items, this.props.t);
            }
        } else {
            const result = move(
                this.getList(source.droppableId),
                this.getList(destination.droppableId),
                source,
                destination,
                this.props.search,
            );

            state = {
                items: sortFunc(result.droppable, this.props.t),
                selected: result.droppable2,
            };
        }

        this.setState(state);
        this.props.onChange(
            state.selected.map(x => x.content),
            state.items.map(x => x.id),
            this.props.type,
        );
    };

    render() {
        const { t, search } = this.props;

        return (
            <DragDropContext onDragEnd={this.onDragEnd}>
                <DroppableLabel
                    items={this.state.items}
                    search={search}
                    droppableId="droppable"
                    name={t('Available')}
                    t={t}
                />
                <DroppableLabel
                    items={this.state.selected}
                    search={search}
                    droppableId="droppable2"
                    name={t('Selected')}
                    t={t}
                />
            </DragDropContext>
        );
    }
}

const DroppableLabel = ({ items, droppableId, name, t, search }) => (
    <div style={{ width: '49%' }}>
        <p style={{ textAlign: 'center' }}>{name}</p>
        <Droppable droppableId={droppableId}>
            {(provided, snapshot) => (
                <div
                    ref={provided.innerRef}
                    style={{
                        padding: 5,
                        width: '100%',
                        border: '1px solid #eee',
                        height: 450,
                        overflowY: 'auto',
                    }}
                >
                    {items.map((item, index) => (
                        <>
                            {t(item.id)
                                .toLowerCase()
                                .includes(search.toLowerCase()) ? (
                                <Draggable key={item.id} draggableId={item.id} index={index}>
                                    {(provided, snapshot) => (
                                        <div
                                            ref={provided.innerRef}
                                            {...provided.draggableProps}
                                            {...provided.dragHandleProps}
                                            style={getItemStyle(
                                                snapshot.isDragging,
                                                provided.draggableProps.style,
                                            )}
                                        >
                                            <Label style={{ width: '100%' }}>
                                                {t(item.content.name)}
                                            </Label>
                                        </div>
                                    )}
                                </Draggable>
                            ) : null}
                        </>
                    ))}
                    {provided.placeholder}
                </div>
            )}
        </Droppable>
    </div>
);

export default DragAndDropFields;
