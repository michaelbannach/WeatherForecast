import { useState } from 'react';

function App() {
    const [city, setCity] = useState('');
    const [country, setCountry] = useState('');
    const [result, setResult] = useState('');
    const [error, setError] = useState('');

    const fetchWeather = async () => {
        setError('');
        setResult('');
        try {
            const response = await fetch(`http://localhost:5268/api/weather/${city}/${country}`);
            if (!response.ok) {
                setError('Backend-Antwort war fehlerhaft');
                return;
            }
            const data = await response.json();
            setResult(`Temperatur: ${data.main.temp} °C, Wetter: ${data.weather[0].description}`);
        } catch (e) {
            setError('Verbindung fehlgeschlagen');
        }
    };

    return (
        <div>
            <h2>Wetter Abfrage</h2>
            <input
                type="text"
                placeholder="Stadt"
                value={city}
                onChange={(e) => setCity(e.target.value)}
            />
            <input
                type="text"
                placeholder="Land (z.B. DE)"
                value={country}
                onChange={(e) => setCountry(e.target.value)}
                maxLength={2}
            />
            <button onClick={fetchWeather}>Suchen</button>

            <div style={{ marginTop: 20 }}>
                {result && <div>Ergebnis: {result}</div>}
                {error && <div style={{ color: 'red' }}>{error}</div>}
            </div>
        </div>
    );
}

export default App;
