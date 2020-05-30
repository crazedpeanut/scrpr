import React, { useEffect, useState } from "react";

const baseUrl = "http://localhost:5000";

interface Entity {
  name: string;
  raw: string;
  source: string;
}
interface ScraperResult {
  entities: Entity[];
  id: string;
}
interface ScraperJob {
  url: string;
  id: string;
  status: number;
  error: string;
}

const scheduleScraper = (url: string) =>
  fetch(`${baseUrl}/scraperjobs`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      url,
    }),
  });

const getScraperJobs = (): Promise<ScraperJob[]> =>
  fetch(`${baseUrl}/scraperjobs`, {
    method: "GET",
  }).then((r) => {
    if (r.status !== 200) {
      return null;
    }
    return r.json();
  });

const getResults = (url: string): Promise<ScraperResult[]> =>
  fetch(`${baseUrl}/scraperresults/${encodeURIComponent(url)}`, {
    method: "GET",
  }).then((r) => {
    if (r.status !== 200) {
      return null;
    }
    return r.json();
  });

function App() {
  const [jobs, setJobs] = useState<ScraperJob[]>([]);
  const [url, setUrl] = useState(
    "https://www.acma.gov.au/choose-your-phone-number"
  );
  const [results, setResults] = useState<ScraperResult[]>();

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
              <button
                onClick={() => onGetResults(job.url)}
                disabled={job.status !== 2}
              >
                Go
              </button>
            </li>
          ))}
        </ul>
      </div>
      <div>
        Results <br />
        <ul>
          {results &&
            results.map((result) => (
              <li key={result.id}>
                {result.id}
                <ul>
                  {result.entities.map((entity, i) => (
                    <li key={i}>
                      Name: {entity.name}, raw: {entity.raw}, source:{" "}
                      {entity.source}
                    </li>
                  ))}
                </ul>
              </li>
            ))}
        </ul>
      </div>
    </div>
  );

  async function onStart() {
    await scheduleScraper(url);
  }

  async function onGetResults(url: string) {
    const results = await getResults(url);
    setResults(results);
  }
}

export default App;
