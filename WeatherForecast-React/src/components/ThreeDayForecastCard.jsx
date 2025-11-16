import React from "react";
import { Sun, Cloud, CloudRain, Snowflake, CloudSun } from "lucide-react";

export function ThreeDayForecastCard({ forecast3Days }) {
    if (!forecast3Days || forecast3Days.length === 0) return null;

    // Platzhalter, damit alle fünf Grid-Spalten gefüllt sind:
    const emptyCards = Array.from({ length: 5 - forecast3Days.length });

    return (
        <section className="grid grid-cols-3 md:grid-cols-3 gap-3 w-full">
            {forecast3Days.map((day, idx) => {
                const owmIconUrl = day.icon
                    ? `https://openweathermap.org/img/wn/${day.icon}@4x.png`
                    : null;

                const descr = (day.description || "").toLowerCase();
                let FallbackIcon = CloudSun;
                if (descr.includes("sun") || descr.includes("klar")) FallbackIcon = Sun;
                else if (descr.includes("cloud") || descr.includes("wolk")) FallbackIcon = Cloud;
                else if (descr.includes("rain") || descr.includes("regen")) FallbackIcon = CloudRain;
                else if (descr.includes("snow") || descr.includes("schnee")) FallbackIcon = Snowflake;

                return (
                    <div
                        key={idx}
                        className="bg-white border border-slate-200 rounded-xl p-3 shadow-sm flex flex-col gap-1 items-center text-xs text-center justify-center"
                    >
                        <p className="text-[10px] font-semibold text-slate-600 uppercase text-center">
                            {day.label}
                        </p>
                        {owmIconUrl ? (
                            <img
                                src={owmIconUrl}
                                alt={day.description}
                                className="mx-auto w-10 h-10"
                                loading="lazy"
                            />
                        ) : (
                            <FallbackIcon className="mx-auto w-10 h-10 text-slate-400" />
                        )}
                        <p className="text-sm font-semibold text-slate-900 text-center">
                            {day.max}°
                        </p>
                      
                      
                    </div>
                );
            })}
            {emptyCards.map((_, idx) => (
                <div key={`empty-${idx}`} className="invisible"></div>
            ))}
        </section>
    );
}
