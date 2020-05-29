import React, { useEffect, useState } from "react";

const baseUrl = "http://localhost:5000/scraperjobs";

interface Entity {
  name: string;
  raw: string;
  source: string;
}
interface ScraperResult {
  entities: Entity[];
}

const scheduleScraper = (url: string) =>
  fetch(`${baseUrl}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      url,
    }),
  });

const getScraperResult = (): Promise<ScraperResult | null> =>
  fetch(`${baseUrl}`, {
    method: "GET",
  }).then((r) => {
    if (r.status !== 200) {
      return null;
    }
    return r.json();
  });

const Result: React.FC<{ result: ScraperResult }> = ({ result }) => {
  return (
    <div>
      {result.entities.map((e, i) => (
        <div key={i}>
          Name: {e.name}, Value: {e.raw}, Source: {e.source}
        </div>
      ))}
    </div>
  );
};

function App() {
  const [result, setResult] = useState<ScraperResult | null>(null);
  const [url, setUrl] = useState("");

  useEffect(() => {
    getScraperResult().then(setResult);
  }, []);

  return (
    <div>
      <div>
        <input
          type="url"
          value={url}
          onChange={(e) => setUrl(e.target.value)}
        />
        <button onClick={onStart}>Start!</button>
      </div>
      <div>
        Results! <br/>
        {result ? <Result result={result} /> : "Loading"}
      </div>
    </div>
  );

  async function onStart() {
    await scheduleScraper(url);
  }
}

export default App;
