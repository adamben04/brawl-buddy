// TODO: Implement routing and main layout

import { useState, useEffect } from 'react';
import axios from 'axios';

// Try multiple backend URLs in order of preference
const API_URLS = [
  'https://localhost:5001',
  'http://localhost:5000', 
  'http://localhost:5001'
];

interface TestResponse {
  message: string;
  timestamp: string;
}

function App() {
  const [message, setMessage] = useState('Connecting to backend...');
  const [backendStatus, setBackendStatus] = useState('pending');
  const [connectedUrl, setConnectedUrl] = useState('');

  useEffect(() => {
    const tryConnectToBackend = async () => {
      for (const apiUrl of API_URLS) {
        try {
          console.log(`Trying to connect to: ${apiUrl}`);
          const response = await axios.get<TestResponse>(`${apiUrl}/api/player/test`, {
            timeout: 3000 // 3 second timeout
          });
          
          setMessage(response.data.message);
          setBackendStatus('success');
          setConnectedUrl(apiUrl);
          console.log(`Successfully connected to: ${apiUrl}`);
          return; // Exit loop on success
        } catch (error) {
          console.log(`Failed to connect to ${apiUrl}:`, error);
        }
      }
      
      // If we get here, all URLs failed
      setMessage('Failed to connect to backend. Please start the API server.');
      setBackendStatus('error');
    };

    tryConnectToBackend();
  }, []);

  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center justify-center font-sans p-4">
      <header className="text-center">
        <h1 className="text-5xl font-bold mb-4">
          Brawl Buddy
        </h1>
        <p className="text-lg text-gray-400 mb-8">
          Your ultimate Brawl Stars companion.
        </p>
        <div className="status-card bg-gray-800 p-6 rounded-lg shadow-lg max-w-md mx-auto">
          <h2 className="text-2xl font-semibold mb-3">Application Status</h2>          <div className="flex items-center justify-center">
            <span className={`h-4 w-4 rounded-full mr-3 ${
              backendStatus === 'success' ? 'bg-green-500' : backendStatus === 'error' ? 'bg-red-500' : 'bg-yellow-500 animate-pulse'
            }`}></span>
            <p>
              Backend Status: <span className="font-semibold ml-1">{message}</span>
            </p>
          </div>
          {connectedUrl && (
            <p className="text-sm text-gray-400 mt-2">
              Connected to: {connectedUrl}
            </p>
          )}
        </div>
      </header>
    </div>
  )
}

export default App