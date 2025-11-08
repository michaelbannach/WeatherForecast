import { useState } from 'react';

const API_BASE = 'http://localhost:5278';

function App() {
    const [city, setCity] = useState('');
    const [country, setCountry] = useState('');
    const [result, setResult] = useState(null);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const fetchWeather = async () => {
        setError('');
        setResult(null);

        const c = city.trim();
        const co = country.trim().toUpperCase();

        if (!c) { setError('Bitte eine Stadt angeben.'); return; }
        if (co.length !== 2) { setError('Ländercode muss 2-stellig sein (z. B. DE).'); return; }

        setLoading(true);
        try {
            const url = `${API_BASE}/api/weather/${encodeURIComponent(c)}/${encodeURIComponent(co)}`;
            const response = await fetch(url, { headers: { 'Accept': 'application/json' } });

            // Fehlertext vom Backend anzeigen (400/404 liefern Text)
            if (!response.ok) {
                const text = await response.text();
                setError(text || 'Backend-Antwort war fehlerhaft');
                return;
            }

            const data = await response.json();
            // Achtung: passt zum WeatherDto (camelCase)
            // { city, country, temp, feelsLike, tempMin, tempMax, humidity, windSpeed, sunrise, sunset, summary, description, icon }
            setResult(data);
        } catch (e) {
            setError('Verbindung fehlgeschlagen');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: 520, margin: '2rem auto', fontFamily: 'system-ui, sans-serif' }}>
            <h2>Wetter Abfrage</h2>

            <div style={{ display: 'grid', gap: 8, gridTemplateColumns: '1fr 120px auto' }}>
                <input
                    type="text"
                    placeholder="Stadt"
                    value={city}
                    onChange={(e) => setCity(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && fetchWeather()}
                />
                <input
                    type="text"
                    placeholder="Land (z. B. DE)"
                    value={country}
                    onChange={(e) => setCountry(e.target.value)}
                    maxLength={2}
                    onKeyDown={(e) => e.key === 'Enter' && fetchWeather()}
                />
                <button onClick={fetchWeather} disabled={loading}>
                    {loading ? 'Lade…' : 'Suchen'}
                </button>
            </div>

            {/* Speichern geht nur mit Auth, Favorite-API ist [Authorize].
          Bis Login/Cookies stehen, lassen wir den Button als Platzhalter. */}
            <div style={{ marginTop: 8 }}>
                <button onClick={() => alert('Speichern erfordert Login (Favorite-API ist geschützt).')}>
                    Speichern
                </button>
            </div>

            <div style={{ marginTop: 20 }}>
                {result && (
                    <div style={{ padding: 12, border: '1px solid #ddd', borderRadius: 8 }}>
                        <div><strong>{result.city}, {result.country}</strong></div>
                        <div>Temperatur: {Math.round(result.temp)} °C (gefühlt {Math.round(result.feelsLike)} °C)</div>
                        <div>Wetter: {result.description || result.summary}</div>
                        {result.icon && (
                            <img
                                alt={result.description || 'Wetter-Icon'}
                                src={`https://openweathermap.org/img/wn/${result.icon}@2x.png`}
                                style={{ verticalAlign: 'middle' }}
                            />
                        )}
                    </div>
                )}
                {error && <div style={{ color: 'red' }}>{error}</div>}
            </div>
        </div>
    );
}

export default App;
