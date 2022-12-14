import axios from "axios";
import IPostBody from '../types/IPostBody'

const baseURL = process.env.REACT_APP_BASE_URL;

const postImg = async (data:IPostBody) => {
    console.log(data)
    let result = await axios.post(`${baseURL}/img`, data)
    return result.data;
} 

const fileService = {
    postImg
}

export default fileService;