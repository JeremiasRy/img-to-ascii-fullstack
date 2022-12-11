import axios from "axios";

const baseURL = process.env.REACT_APP_BASE_URL;

const postImg = async (data:FormData) => {
    let result = await axios.post(`${baseURL}/img`, data)
    console.log(result);
} 

export default { postImg };