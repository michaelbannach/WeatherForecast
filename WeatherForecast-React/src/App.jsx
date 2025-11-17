
import React, { useState } from "react";
import { Routes, Route } from "react-router-dom";
import { SidebarProvider } from "@/components/ui/sidebar";

import { AppSidebar } from "./components/AppSidebar";
import DashboardPage from "./components/DashboardPage";
import LoginPage from "./components/LoginPage";
import RegisterPage from "./components/RegisterPage";
import { CurrentWeatherCard } from "./components/CurrentWeatherCard";
import { ThreeDayForecastCard } from "./components/ThreeDayForecastCard";
import { FiveDayForecastCard } from "./components/FiveDayForecastCard";
import { SearchCard } from "./components/SearchCard";
import WeatherDetailsCards from "./components/WeatherDetailsCards";

const API_BASE = "http://localhost:5278";

export default function App() {
    const [city, setCity] = useState("");
    const [country, setCountry] = useState("");
    const [weather, setWeather] = useState(null);
    const [forecast3Days, setForecast3Days] = useState([]);
    const [forecast5Days, setForecast5Days] = useState([]);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    // Wetter für die Hauptseite (Suche) laden
    const loadWeather = async (c, co) => {
        setError("");
        setWeather(null);
        setForecast3Days([]);
        setForecast5Days([]);

        setCity(c);
        setCountry(co);

        setLoading(true);

        try {
            const respWeather = await fetch(
                `${API_BASE}/api/weather/${encodeURIComponent(c)}/${encodeURIComponent(co)}`
            );
            if (!respWeather.ok) throw new Error(await respWeather.text());
            const weatherData = await respWeather.json();
            setWeather(weatherData);

            const resp3 = await fetch(
                `${API_BASE}/api/weather/forecast/3days/${encodeURIComponent(c)}/${encodeURIComponent(co)}`
            );
            const threeDays = await resp3.json();
            setForecast3Days(
                threeDays.map((d) => ({
                    label: new Date(d.date).toLocaleDateString("de-DE", {
                        weekday: "short",
                    }),
                    min: d.tempMin,
                    max: d.tempMax,
                    description: d.description,
                    icon: d.icon,
                }))
            );

            const resp5 = await fetch(
                `${API_BASE}/api/weather/forecast/5days/${encodeURIComponent(c)}/${encodeURIComponent(co)}`
            );
            const fiveDays = await resp5.json();
            setForecast5Days(
                fiveDays.map((d) => ({
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

    const handleSearch = async (e) => {
        e.preventDefault();

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

        await loadWeather(c, co);
    };

    const handleAddToFavorites = async () => {
        if (!weather) return;

        try {
            const resp = await fetch(`${API_BASE}/api/favorite`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({
                    city: weather.city,
                    country: weather.country,
                }),
            });

            if (!resp.ok) throw new Error(await resp.text());

            alert("Ort als Favorit gespeichert!");
        } catch (err) {
            alert(err.message);
        }
    };

    return (
        <SidebarProvider>
            <div className="min-h-screen bg-slate-100 text-slate-900 flex">
                <AppSidebar />

                <main className="flex-1 ml-64 flex items-start justify-center p-10">
                    <div className="w-full max-w-3xl space-y-6">
                        <Routes>
                            {/* Wetterseite */}
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
                                                <WeatherDetailsCards weather={weather} />
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

                                      
                                    </>
                                }
                            />

                            
                            <Route path="/login" element={<LoginPage />} />
                            <Route path="/register" element={<RegisterPage />} />

                           
                            <Route path="/dashboard" element={<DashboardPage />} />
                        </Routes>
                    </div>
                </main>
            </div>
        </SidebarProvider>
    );
}
