import axios from 'axios';
import qs from 'qs';
import { toast } from 'react-toastify';

export const postman = axios.create({
    baseURL: '/api',
    paramsSerializer: params => qs.stringify(params, { indices: false }),
});

postman.interceptors.response.use(
    resp => {
        return resp.data;
    },
    error => {
        const { data = {} } = error.response;
        const { error: errorText = '', message = '' } = data;
        console.log('data', errorText);
        errorText && toast.error(JSON.stringify(errorText) || message || 'Ошибка!');

        return Promise.reject(error);
    },
);

export const setAccessToken = token => {
    if (token !== null) {
        postman.defaults.headers.common.Authorization = `Bearer ${token}`;
    } else {
        delete postman.defaults.headers.common.Authorization;
    }
};
