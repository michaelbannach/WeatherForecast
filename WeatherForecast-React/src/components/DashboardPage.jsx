import React, { useEffect, useState } from "react";
import FavoriteStrip from "./FavoriteStrip";
import { CurrentWeatherCard } from "./CurrentWeatherCard";
import SunriseSunsetCards from "./SunriseSunsetCards";
import { FiveDayForecastCard } from "./FiveDayForecastCard";
import WeatherDetailsCards from "./WeatherDetailsCards";

const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5000";

const getAuthHeaders = () => {
    const token = localStorage.getItem("jwt");
    return token
        ? { Authorization: `Bearer ${token}` }
        : null;
};

export default function DashboardPage() {
    const [favorites, setFavorites] = useState([]);
    const [favoritesLoading, setFavoritesLoading] = useState(true);
    const [favoritesError, setFavoritesError] = useState("");

    
    const [weather, setWeather] = useState(null);
    const [forecast3Days, setForecast3Days] = useState([]);
    const [forecast5Days, setForecast5Days] = useState([]);
    const [weatherLoading, setWeatherLoading] = useState(false);
    const [weatherError, setWeatherError] = useState("");

    // Load Favorites
    const loadFavorites = async () => {
        setFavoritesLoading(true);
        setFavoritesError("");

        try {
            const authHeaders = getAuthHeaders();
            if (!authHeaders) {
                setFavoritesError("Bitte zuerst einloggen, um Favoriten zu laden.");
                setFavoritesLoading(false);
                return;
            }

            const resp = await fetch(`${API_BASE}/api/favorite`, {
                method: "GET",
                headers: {
                    ...authHeaders,
                },
            });

            if (!resp.ok) {
                const msg = await resp.text();
                throw new Error(msg || "Fehler beim Laden der Favoriten");
            }

            const data = await resp.json();
            setFavorites(data);
        } catch (err) {
            setFavoritesError(err.message || "Fehler beim Laden der Favoriten");
        } finally {
            setFavoritesLoading(false);
        }
    };


    useEffect(() => {
        loadFavorites();
    }, []);

    //  Load Weather clicked Favorite
    const loadWeatherForFavorite = async (city, country) => {
        if (!city || !country) return;

        setWeather(null);
        setForecast3Days([]);
        setForecast5Days([]);
        setWeatherError("");
        setWeatherLoading(true);

        const c = city.trim();
        const co = country.trim().toUpperCase();

        try {
            // Current Weather
            const respWeather = await fetch(
                `${API_BASE}/api/weather/${encodeURIComponent(c)}/${encodeURIComponent(co)}`
            );
            if (!respWeather.ok) {
                const msg = await respWeather.text();
                throw new Error(msg || "Fehler bei der Abfrage des aktuellen Wetters.");
            }
            const weatherData = await respWeather.json();
            setWeather(weatherData);

            // 3-Days-Forecast
            const resp3 = await fetch(
                `${API_BASE}/api/weather/forecast/3days/${encodeURIComponent(c)}/${encodeURIComponent(co)}`
            );
            const threeData = await resp3.json();
            setForecast3Days(
                threeData.map((d) => ({
                    label: new Date(d.date).toLocaleDateString("de-DE", {
                        weekday: "short",
                    }),
                    min: d.tempMin,
                    max: d.tempMax,
                    description: d.description,
                    icon: d.icon,
                }))
            );

            // 5-Days-Forecast
            const resp5 = await fetch(
                `${API_BASE}/api/weather/forecast/5days/${encodeURIComponent(c)}/${encodeURIComponent(co)}`
            );
            const fiveData = await resp5.json();
            setForecast5Days(
                fiveData.map((d) => ({
                    label: new Date(d.date).toLocaleDateString("de-DE", {
                        weekday: "short",
                    }),
                    min: d.tempMin,
                    max: d.tempMax,
                    description: d.description,
                    icon: d.icon,
                }))
            );
        } catch (err) {
            setWeatherError(err.message || "Fehler beim Abrufen der Wetterdaten.");
        } finally {
            setWeatherLoading(false);
        }
    };

    // Delete Favorite
    const handleRemove = async (id) => {
        try {
            const authHeaders = getAuthHeaders();
            if (!authHeaders) {
                alert("Bitte zuerst einloggen, um Favoriten zu löschen.");
                return;
            }

            const resp = await fetch(`${API_BASE}/api/favorite/${id}`, {
                method: "DELETE",
                headers: {
                    ...authHeaders,
                },
            });

            if (!resp.ok) {
                const msg = await resp.text();
                throw new Error(msg || "Fehler beim Löschen des Favoriten");
            }

            setFavorites((prev) => prev.filter((f) => f.id !== id));
        } catch (err) {
            alert(err.message);
        }
    };


    return (
        <div className="space-y-6">
            <header>
                <h1 className="text-4xl font-semibold">Dashboard</h1>
                <p className="text-sm text-slate-500">
                    
                </p>
            </header>

            {favoritesLoading && (
                <p className="text-sm text-slate-500">Lade Favoriten…</p>
            )}
            {favoritesError && (
                <p className="text-sm text-red-500">{favoritesError}</p>
            )}

            {!favoritesLoading && !favoritesError && favorites.length > 0 && (
                <FavoriteStrip
                    favorites={favorites}
                    onRemove={handleRemove}
                    onSelect={loadWeatherForFavorite} // ⭐ Klick lädt Wetter auf dieser Seite
                />
            )}

            {!favoritesLoading && !favoritesError && favorites.length === 0 && (
                <p className="text-xs text-slate-500">
                    Du hast noch keine Favoriten. Füge auf der Wetterseite welche hinzu.
                </p>
            )}

            {/* Bereich für Wetterdetails unter den Favoriten */}
            {weatherLoading && (
                <p className="text-sm text-slate-500 mt-4">
                    Lade Wetterdetails…
                </p>
            )}

            {weatherError && (
                <p className="text-sm text-red-500 mt-4">{weatherError}</p>
            )}

            {weather && (
                <div className="space-y-4 mt-4">
                    <CurrentWeatherCard weather={weather} />
                    <WeatherDetailsCards weather={weather} />
                    <SunriseSunsetCards weather={weather} />
                    <FiveDayForecastCard forecast5Days={forecast5Days} />
                </div>
            )}
        </div>
    );
}
