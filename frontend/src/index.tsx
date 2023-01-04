import React from 'react';
import { createRoot } from 'react-dom/client';
import * as Sentry from "@sentry/react";
import './index.css';
import App from './components/App/App';
import * as serviceWorker from './serviceWorker';

Sentry.init({
    dsn: "https://e560ecb7fbd44b6a91809cf7deadaa4e@o247191.ingest.sentry.io/5536424",
    release: "dvb-live-frontend@" + process.env.npm_package_version,
    autoSessionTracking: true,
    enabled: process.env.NODE_ENV === 'production',
});

const container = document.getElementById('root');
const root = createRoot(container!);
root.render(
  <React.StrictMode>
      <Sentry.ErrorBoundary fallback={<p>An error has occurred</p>} showDialog>
          <App />
      </Sentry.ErrorBoundary>
  </React.StrictMode>
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
