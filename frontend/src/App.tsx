import { useState, useRef, SyntheticEvent, useEffect } from "react";
import fileService from "./services/fileService";
import "./App.css";
import Picture from "./types/Picture";

interface IOutputelement {
  width:number,
  height:number
}
function App() {

  const [ picture, setPicture ] = useState<Picture | null>(null);
  const [ file, setFile ] = useState<Blob | null>(null);
  const [ divClassName, setDivClassName ] = useState<"max-size" | "fit-picture">("max-size");
  const [ outputElement, setOutputElement ] = useState<IOutputelement | undefined>(undefined);
  const [ fromUrl, setFromUrl ] = useState<boolean>(false);
  const [ url, setUrl ] = useState<string>("");
  const element = useRef(null);

  
  useEffect(() => {
    if (element.current !== null && outputElement === undefined) {
      let styles:CSSStyleDeclaration;
      styles = getComputedStyle(element.current);
      let outputElement:IOutputelement = {
        width: Math.floor(parseInt(styles.width.replace('px', '')) / parseInt(styles.fontSize.replace('px', ''))),
        height: Math.floor(parseInt(styles.height.replace('px', '')) / 10)
      }
      setOutputElement(outputElement);
    }
  }, [outputElement]) ;
  
  async function handleSubmit(e:SyntheticEvent) {
    e.preventDefault();
    if (fromUrl) {
      let res = await fetch(url)
      setFile(await res.blob())
    }
    if (element.current !== null && file !== null && outputElement !== undefined) { 
      const imgFormData = new FormData();
      imgFormData.append("image", file);
      imgFormData.append("width", String(outputElement.width));
      imgFormData.append("height", String(outputElement.height))
      let imageData = await fileService.postImg(imgFormData);
      setPicture(new Picture(imageData.height, imageData.width, imageData.rows));
      setFile(null);
      setDivClassName("fit-picture");
    }  
  }

  return (
    <div className="App">
      <h1>Img to ASCII art converter</h1>
      <div className="form-wrapper">
        <form className="form" onSubmit={handleSubmit}>
          <button onClick={() => setFromUrl(!fromUrl)}>{fromUrl ? "From your computer" : "From url"}</button>
          {fromUrl 
            ? <input 
            type="url" 
            id="pic-url"
            placeholder="Put direct url to the picture"
            onChange={(e) => setUrl(e.currentTarget.value)}/>
            : <input 
            id="picture-to-submit" 
            type="file" 
            onChange={(e) => {
              if (e.target.files !== null) {
                setFile(e.target.files[0] as Blob)
              } else {
              //set notifcation element
              }}}/>
          }
          
          <input type="submit" className="submit-button"/>
        </form>
      </div>
      <div id="picture" className={divClassName} ref={element}>
        {picture !== null && picture.picture.map(row => <>{row}<br/></>)}
      </div>
    </div>
  )
};

export default App;
