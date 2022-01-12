import React from 'react';
import logo from '../../assets/logo.svg';
import './App.css';
import Map from '../Map/Map';
import { QueryClient, QueryClientProvider } from 'react-query';
import { persistQueryClient } from 'react-query/persistQueryClient-experimental'
import { createWebStoragePersistor } from 'react-query/createWebStoragePersistor-experimental';
import { ReactQueryDevtools } from 'react-query/devtools';

// BEGIN React Query Setup
const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            cacheTime: 1000 * 60 * 60 * 24 // 24 hours
        }
    }
});
const webStoragePersistor = createWebStoragePersistor({ storage: window.localStorage })

persistQueryClient({
    queryClient,
    persistor: webStoragePersistor,
});
// END React Query Setup

function App() {
  return (
    <QueryClientProvider client={queryClient}>
        <Page />
        <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}

function Page() {
    return (
        <div className="App">
            <header className="App-header">
                <img src={logo} className="App-logo" alt="logo" />
                <p>
                    Edit <code>src/App.tsx</code> and save to reload.
                </p>
                <a
                    className="App-link"
                    href="https://reactjs.org"
                    target="_blank"
                    rel="noopener noreferrer"
                >
                    Learn React
                </a>
            </header>
            <Map />
        </div>
    );
}

export default App;
