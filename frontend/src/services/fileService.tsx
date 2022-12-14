import axios from "axios";

const baseURL = process.env.REACT_APP_BASE_URL;

const postImg = async (data:FormData) => {
    console.log(data)
    let result = await axios.post(`${baseURL}/img`, data)
    return result.data;
} 

const fileService = {
    postImg
}

export default fileService;