import axios from 'axios';
import qs from 'qs';

export const postman = axios.create({
    baseURL: '',
    paramsSerializer: params => qs.stringify(params, { indices: false }),
});

postman.interceptors.response.use(
    resp => {
        return resp.data;
    },
    error => {
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
