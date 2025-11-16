import React from "react";

export default function WeatherDetailsCards({ weather }) {
    if (!weather) return null;

    return (
        <div className="grid grid-cols-3 gap-4 mt-6">
            <div className="bg-white rounded-xl p-5 shadow-sm border border-slate-200">
                <p className="text-xs font-medium text-slate-500 uppercase tracking-wide text-center">
                    Temperatur
                </p>
                <p className="text-2xl font-semibold text-slate-800 text-center">
                    {weather.temp.toFixed(1)}°C
                </p>
                <p className="text-xs text-slate-500 mt-1 text-center">
                    gefühlt {weather.feelsLike.toFixed(1)}°C
                </p>
            </div>

            <div className="bg-white rounded-xl p-5 shadow-sm border border-slate-200 text-center">
                <p className="text-xs font-medium text-slate-500 uppercase tracking-wide text-center">
                    Luftfeuchtigkeit
                </p>
                <p className="text-2xl font-semibold text-slate-800 text-center">
                    {weather.humidity}%
                </p>
            </div>

            <div className="bg-white rounded-xl p-5 shadow-sm border border-slate-200 text-center">
                <p className="text-xs font-medium text-slate-500 uppercase tracking-wide text-center">
                    Windgeschwindigkeit
                </p>
                <p className="text-2xl font-semibold text-slate-800 text-center">
                    {weather.windSpeed.toFixed(1)} m/s
                </p>
            </div>
        </div>
    );
}
