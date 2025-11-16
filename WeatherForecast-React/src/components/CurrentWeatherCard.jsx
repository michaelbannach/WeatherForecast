import React from "react";
import {
    Thermometer,
    Cloud,
    CloudSun,
    CloudRain,
    Sun,
    Snowflake,
} from "lucide-react";

export function CurrentWeatherCard({ weather }) {
    if (!weather) return null;

    const {
        city,
        country,
        temp = 0,
        feelsLike = 0,
        summary = "",
        description = "",
        icon = "",
    } = weather;

    // Wenn OpenWeatherMap Icon vorhanden, verwenden
    const owmIconUrl = icon
        ? `https://openweathermap.org/img/wn/${icon}@4x.png`
        : null;

    // Fallback für Icons (wenn kein OWM-Icon vorhanden)
    const FallbackIcon = (() => {
        const key = (summary || description || "").toLowerCase();
        if (key.includes("sun") || key.includes("klar")) return Sun;
        if (key.includes("cloud")) return Cloud;
        if (key.includes("rain") || key.includes("regen")) return CloudRain;
        if (key.includes("snow") || key.includes("schnee")) return Snowflake;
        return CloudSun;
    })();

    return (
        <section className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm flex flex-col items-center text-center">
            {/* Stadtname */}
            <div className="mb-2">
                <h3 className="text-xl font-semibold leading-tight">
                    {city}, {country}
                </h3>
                {(description || summary) && (
                    <p className="text-sm text-slate-500">
                        {description || summary}
                    </p>
                )}
            </div>

            {/* Icon */}
            <div className="mt-3 mb-4">
                {owmIconUrl ? (
                    <img
                        src={owmIconUrl}
                        alt={summary || "Wetter"}
                        className="w-28 h-28 md:w-32 md:h-32 object-contain drop-shadow-sm"
                        loading="lazy"
                    />
                ) : (
                    <FallbackIcon className="w-28 h-28 md:w-32 md:h-32 text-slate-500" />
                )}
            </div>

            {/* Temperatur + "gefühlt" */}
            <div className="flex flex-col items-center">
                <div className="flex items-center gap-2">
                    <Thermometer className="w-5 h-5 text-slate-500" />
                    <span className="text-6xl font-semibold leading-none">
            {Math.round(temp)}°
          </span>
                </div>
                <p className="text-sm text-slate-500 mt-1">
                    gefühlt {Math.round(feelsLike)}°
                </p>
            </div>
        </section>
    );
}
