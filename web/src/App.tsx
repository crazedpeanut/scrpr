import { config } from './services/config';

import React, { useEffect, useState } from 'react';

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

const scheduleScraper = (url: string): Promise<Response> =>
  fetch(`${config.apiBaseUrl}/scraperjobs`, {
    body: JSON.stringify({
      url
    }),
    headers: {
      'Content-Type': 'application/json'
    },
    method: 'POST'
  });

const getScraperJobs = (): Promise<ScraperJob[]> =>
  fetch(`${config.apiBaseUrl}/scraperjobs`, {
    method: 'GET'
  }).then((r) => {
    if (r.status !== 200) {
      return null;
    }
    return r.json();
  });

const getResults = (url: string): Promise<ScraperResult[]> =>
  fetch(`${config.apiBaseUrl}/scraperresults/${encodeURIComponent(url)}`, {
    method: 'GET'
  }).then((r) => {
    if (r.status !== 200) {
      return null;
    }
    return r.json();
  });

const App: React.FC = () => {
  const [jobs, setJobs] = useState<ScraperJob[]>([]);
  const [targetUrl, setTargetUrl] = useState('https://www.acma.gov.au/choose-your-phone-number');
  const [results, setResults] = useState<ScraperResult[]>();

  async function onStart(): Promise<void> {
    await scheduleScraper(targetUrl);
  }

  async function onGetResults(url: string): Promise<void> {
    setResults(await getResults(url));
  }

  useEffect(() => {
    const interval = setInterval(() => {
      getScraperJobs().then(setJobs);
    }, 500);

    return () => clearInterval(interval);
  }, []);

  return (
    <div>
      <div>
        <input type="url" value={targetUrl} onChange={(e) => setTargetUrl(e.target.value)} />
        <button type="button" onClick={onStart}>
          Start!
        </button>
      </div>
      <div>
        Jobs! <br />
        <ul>
          {jobs.map((job) => (
            <li key={job.id}>
              Id: {job.id}, Url: {job.url}, status: {job.status}
              <button
                disabled={job.status !== 2}
                type="button"
                onClick={() => onGetResults(job.url)}
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
                    // eslint-disable-next-line react/no-array-index-key
                    <li key={i}>
                      Name: {entity.name}, raw: {entity.raw}, source: {entity.source}
                    </li>
                  ))}
                </ul>
              </li>
            ))}
        </ul>
      </div>
    </div>
  );
};

export default App;
