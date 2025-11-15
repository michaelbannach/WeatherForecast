import React, { useState } from "react";
import { Routes, Route } from "react-router-dom";
import { SidebarProvider } from "@/components/ui/sidebar";

import { AppSidebar } from "./components/AppSidebar";
import LoginPage from "./components/LoginPage";
import RegisterPage from "./components/RegisterPage";
import { CurrentWeatherCard } from "./components/CurrentWeatherCard";
import { ThreeDayForecastCard } from "./components/ThreeDayForecastCard";
import { FiveDayForecastCard } from "./components/FiveDayForecastCard";
import { SearchCard } from "./components/ui/SearchCard";

const API_BASE = "http://localhost:5278";

export default function App() {
    const [city, setCity] = useState("");
    const [country, setCountry] = useState("");
    const [weather, setWeather] = useState(null);
    const [forecast3Days, setForecast3Days] = useState([]);
    const [forecast5Days, setForecast5Days] = useState([]);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSearch = async (e) => {
        e.preventDefault();
        setError("");
        setWeather(null);
        setForecast3Days([]);
        setForecast5Days([]);

        const c = city.trim();
        const co = country.trim().toUpperCase();

        if (!c) {
            setError("Bitte eine Stadt eingeben.");
            return;
        }
        if (co.length !== 2) {
            setError("Bitte einen 2-stelligen Ländercode (z. B. DE) angeben.");
            return;
        }

        setLoading(true);

        try {
            // Aktuelles Wetter
            const respWeather = await fetch(
                `${API_BASE}/api/weather/${encodeURIComponent(c)}/${encodeURIComponent(co)}`
            );
            if (!respWeather.ok) {
                const msg = await respWeather.text();
                throw new Error(msg || "Fehler bei der Abfrage des aktuellen Wetters.");
            }
            const weatherData = await respWeather.json();
            setWeather(weatherData);

            // 3-Tage-Forecast
            const resp3Days = await fetch(
                `${API_BASE}/api/weather/forecast/3days/${encodeURIComponent(c)}/${encodeURIComponent(co)}`
            );
            if (!resp3Days.ok) {
                const msg = await resp3Days.text();
                throw new Error(msg || "Fehler bei der Abfrage des 3-Tage-Forecasts.");
            }
            const forecast3Data = await resp3Days.json();
            setForecast3Days(
                forecast3Data.map((d) => ({
                    label: new Date(d.date).toLocaleDateString("de-DE", {
                        weekday: "short",
                    }),
                    min: d.tempMin,
                    max: d.tempMax,
                    description: d.description,
                    icon: d.icon,
                }))
            );

            // 5-Tage-Forecast
            const resp5Days = await fetch(
                `${API_BASE}/api/weather/forecast/5days/${encodeURIComponent(c)}/${encodeURIComponent(co)}`
            );
            if (!resp5Days.ok) {
                const msg = await resp5Days.text();
                throw new Error(msg || "Fehler bei der Abfrage des 5-Tage-Forecasts.");
            }
            const forecast5Data = await resp5Days.json();
            setForecast5Days(
                forecast5Data.map((d) => ({
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
            setError(err.message || "Fehler beim Abrufen der Wetterdaten.");
        } finally {
            setLoading(false);
        }
    };

    const handleAddToFavorites = async () => {
        if (!weather) return;

        try {
            const favoriteDto = {
                city: weather.city,
                country: weather.country,
            };

            const resp = await fetch(`${API_BASE}/api/favorite`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
                body: JSON.stringify(favoriteDto),
            });

            if (!resp.ok) {
                const msg = await resp.text();
                throw new Error(msg || "Fehler beim Favoriten hinzufügen.");
            }

            alert("Ort als Favorit gespeichert!");
        } catch (err) {
            alert(err.message);
        }
    };

    return (
        <SidebarProvider>
            <div className="min-h-screen bg-slate-100 text-slate-900 flex">
                <AppSidebar />

                {/* ml-64, damit der Inhalt sauber rechts neben der Sidebar startet */}
                <main className="flex-1 ml-64 flex items-start justify-center p-10">
                    <div className="w-full max-w-3xl space-y-6">
                        <Routes>
                            {/* Startseite: Wettersuche */}
                            <Route
                                path="/"
                                element={
                                    <>
                                        <SearchCard
                                            city={city}
                                            setCity={setCity}
                                            country={country}
                                            setCountry={setCountry}
                                            loading={loading}
                                            onSubmit={handleSearch}
                                            error={error}
                                        />

                                        {weather && (
                                            <>
                                                <CurrentWeatherCard weather={weather} />
                                                <ThreeDayForecastCard forecast3Days={forecast3Days} />
                                                <FiveDayForecastCard forecast5Days={forecast5Days} />

                                                <button
                                                    type="button"
                                                    onClick={handleAddToFavorites}
                                                    className="w-full mt-4 px-4 py-2 rounded bg-blue-600 text-white font-medium hover:bg-blue-700 transition-colors"
                                                >
                                                    Zu Favoriten hinzufügen
                                                </button>
                                            </>
                                        )}

                                        {!weather && !error && (
                                            <p className="text-xs text-slate-500 text-center">
                                                Tipp: Probiere z.&nbsp;B. <strong>Berlin</strong> und{" "}
                                                <strong>DE</strong>.
                                            </p>
                                        )}
                                    </>
                                }
                            />

                            {/* Login & Register */}
                            <Route path="/login" element={<LoginPage />} />
                            <Route path="/register" element={<RegisterPage />} />
                        </Routes>
                    </div>
                </main>
            </div>
        </SidebarProvider>
    );
}
