import { useState, ChangeEvent, useRef } from "react";
import fileService from "./services/fileService";
import "./App.css";
import Picture from "./types/Picture";
import IPostBody from "./types/IPostBody";

function App() {

  const [ picture, setPicture ] = useState<Picture>(new Picture(0,0,[]));
  const element = useRef(null);

  async function handleSubmit(e:ChangeEvent<HTMLInputElement>) {
    e.preventDefault();
    if (e.target.files === null) {
      return;
    }

    if (element.current) { 
      const styles = getComputedStyle(element.current);
      const imgFormData = new FormData();
      imgFormData.append("image", e.target.files[0]);
      
      const body: IPostBody = {
        width: Math.floor(parseInt(styles.width.replace('px', '')) / parseInt(styles.fontSize.replace('px', ''))),
        height: Math.floor(parseInt(styles.height.replace('px', '')) / parseInt(styles.fontSize.replace('px', ''))),
        imageFile: imgFormData,
      }

      let imageData = await fileService.postImg(body);
      setPicture(new Picture(imageData.height, imageData.width, imageData.rows))
    }  
  }

  return (
    <div className="App">
      <h1>jausers</h1>
        <input id="picture-to-submit" type="file" onChange={handleSubmit}/>
        <div id="picture" ref={element}>
          {picture.picture.length !== 0 && picture.picture.map(row => <><span className="row">{row}</span><br/></>)}
        </div>
    </div>
  )
};

export default App;
