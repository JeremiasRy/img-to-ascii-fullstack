import { useState, ChangeEvent, useRef, SyntheticEvent } from "react";
import fileService from "./services/fileService";
import "./App.css";
import Picture from "./types/Picture";

function App() {

  const [ picture, setPicture ] = useState<Picture>(new Picture(0,0,[]));
  const [ file, setFile ] = useState<Blob | null>(null)
  const element = useRef(null);

  async function handleSubmit(e:SyntheticEvent) {
    e.preventDefault();

    if (element.current && file !== null) { 
      const styles = getComputedStyle(element.current);
      const imgFormData = new FormData();

      imgFormData.append("image", file);
      imgFormData.append("width", String(Math.floor(parseInt(styles.width.replace('px', '')) / parseInt(styles.fontSize.replace('px', '')))));
      imgFormData.append("height", String(Math.floor(parseInt(styles.height.replace('px', '')) / parseInt(styles.fontSize.replace('px', '')))))
      
      let imageData = await fileService.postImg(imgFormData);
      setPicture(new Picture(imageData.height, imageData.width, imageData.rows));
      setFile(null);
    }  
  }

  return (
    <div className="App">
      <h1>jausers</h1>
      <form onSubmit={handleSubmit}>
        <input 
          id="picture-to-submit" 
          type="file" 
          onChange={(e) => {
            if (e.target.files !== null) {
              setFile(e.target.files[0] as Blob)
            } else {
              //set notifcation element
            }}}/>
        <input type="submit"/>
      </form>
      <div id="picture" ref={element}>
        {picture.picture.length !== 0 && picture.picture.map(row => <><span className="row">{row}</span><br/></>)}
      </div>
    </div>
  )
};

export default App;
