import { format } from 'date-fns/esm';

export const parseDate = dateString => {
    if (!dateString) return null;
    try {
        const parts = dateString.split('.');
        let d = new Date(parts[2], parts[1] - 1, parts[0]);
        return isNaN(d.getTime()) ? null : d;
    } catch (e) {
        return null;
    }
};

export const dateToString = (date, dateFormat = 'dd.MM.YYYY') => {
    return format(date, dateFormat);
};

export const formatDate = (date, dateFormat = 'dd.MM.YYYY') => {
    if (!date) return null;

    return format(date, dateFormat);
};
