// TODO: Implement API service wrappers
// This file will contain functions to fetch data from the backend API.

import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000', // or https://localhost:5001, depending on your backend configuration
  headers: {
    'Content-Type': 'application/json',
  },
});

export default api;