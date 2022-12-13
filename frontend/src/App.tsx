import { useState, ChangeEvent } from "react";
import fileService from "./services/fileService";
import "./App.css";

class Picture {
    rows: number;
    columns: number;
    picture: string[];

    constructor(rows:number, columns:number, picture:string[]) {
      this.rows = rows;
      this.columns = columns;
      this.picture = picture.map(row => row.replace(/ /g, "\u00A0"));
    }
}

function App() {

  const [ picture, setPicture ] = useState<Picture>(new Picture(0,0,[]));

  async function handleSubmit(e:ChangeEvent<HTMLInputElement>) {
    e.preventDefault();
    if (e.target.files === null) {
      return;
    }
    const imgFormData = new FormData();
    imgFormData.append("image", e.target.files[0]);

    let imageData = await fileService.postImg(imgFormData);
    setPicture(new Picture(imageData.height, imageData.width, imageData.rows))
  }

  if (picture.picture.length !== 0) {
    console.log(picture.picture[77])
  }
  return (
    <div className="App">
      <h1>jausers</h1>
        <input id="picture-to-submit" type="file" onChange={handleSubmit}/>
        <div id="picture">
          {picture.picture.length !== 0 && picture.picture.map(row => <><span className="row">{row}</span><br/></>)};
        </div>
    </div>
  )
};

export default App;
