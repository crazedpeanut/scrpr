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
interface ScraperJob {
  url: string;
  id: string;
  status: number;
  error: string;
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

const getScraperJobs = (): Promise<ScraperJob[]> =>
  fetch(`${baseUrl}`, {
    method: "GET",
  }).then((r) => {
    if (r.status !== 200) {
      return null;
    }
    return r.json();
  });

function App() {
  const [jobs, setJobs] = useState<ScraperJob[]>([]);
  const [url, setUrl] = useState("https://www.acma.gov.au/choose-your-phone-number");

  useEffect(() => {
    const interval = setInterval(() => {
      getScraperJobs().then(setJobs);
    }, 500);

    return () => clearInterval(interval);
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
        Jobs! <br />
        <ul>
          {jobs.map((job) => (
            <li key={job.id}>
              Id: {job.id}, Url: {job.url}, status: {job.status}
            </li>
          ))}
        </ul>
      </div>
    </div>
  );

  async function onStart() {
    await scheduleScraper(url);
  }
}

export default App;
