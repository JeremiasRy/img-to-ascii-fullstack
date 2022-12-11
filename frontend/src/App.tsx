import { ChangeEvent } from "react";
import fileService from "./services/fileService";

function App() {

  function handleSubmit(e:ChangeEvent<HTMLInputElement>) {
    e.preventDefault();
    if (e.target.files === null) {
      return;
    }
    const imgFormData = new FormData();
    imgFormData.append("image", e.target.files[0]);

    fileService.postImg(imgFormData);
  }

  return (
    <div className="App">
      <h1>jausers</h1>
        <input id="picture-to-submit" type="file" onChange={handleSubmit}/>
    </div>
  );
}

export default App;
