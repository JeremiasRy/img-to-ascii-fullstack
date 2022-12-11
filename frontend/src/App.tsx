import { ChangeEvent } from "react";

const fr = new FileReader();

function App() {

  fr.addEventListener('loadend', handleEvent);

  function handleEvent(e:ProgressEvent) {
    console.log(e);
  }

  function handleSubmit(e:ChangeEvent<HTMLInputElement>) {
    e.preventDefault();
    if (e.target.files === null) {
      return;
    }
    fr.readAsArrayBuffer(e.target.files[0]);
  }

  return (
    <div className="App">
      <h1>jausers</h1>
        <input id="picture-to-submit" type="file" onChange={handleSubmit}/>
    </div>
  );
}

export default App;
