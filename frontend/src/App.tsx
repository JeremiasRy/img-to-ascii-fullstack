import { useState, ChangeEvent, useRef, SyntheticEvent } from "react";
import fileService from "./services/fileService";
import "./App.css";
import Picture from "./types/Picture";

function App() {

  const [ picture, setPicture ] = useState<Picture | null>(null);
  const [ file, setFile ] = useState<Blob | null>(null);
  const [ divClassName, setDivClassName ] = useState<"max-size" | "fit-picture">("max-size")
  const element = useRef(null);

  let styles:CSSStyleDeclaration;
  let expectedWidth = "";
  let expectedHeight = "";

  if (element.current !== null) {
    styles = getComputedStyle(element.current);
    expectedHeight = String(Math.floor(parseInt(styles.height.replace('px', '')) / 10));
    expectedWidth = String(Math.floor(parseInt(styles.width.replace('px', '')) / parseInt(styles.fontSize.replace('px', '')))); 
  }

  async function handleSubmit(e:SyntheticEvent) {
    e.preventDefault();

    if (element.current !== null && file !== null) { 

      const imgFormData = new FormData();

      imgFormData.append("image", file);
      imgFormData.append("width", expectedWidth);
      imgFormData.append("height", expectedHeight)
      console.log(Math.floor(parseInt(styles.height.replace('px', '')) / 10))
      let imageData = await fileService.postImg(imgFormData);
      setPicture(new Picture(imageData.height, imageData.width, imageData.rows));
      setFile(null);
      setDivClassName("fit-picture");
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
      <div id="picture" className={divClassName} ref={element}>
        {picture !== null && picture.picture.map(row => <>{row}<br/></>)}
      </div>
    </div>
  )
};

export default App;
