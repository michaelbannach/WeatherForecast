import React from "react";
import { Sun, Cloud, CloudRain, Snowflake, CloudSun } from "lucide-react";

export function FiveDayForecastCard({ forecast5Days }) {
    if (!forecast5Days || forecast5Days.length === 0) return null;

    return (
        <section className="grid grid-cols-1 md:grid-cols-5 gap-3">
            {forecast5Days.map((day, idx) => {
                // OWM-Icon Link, falls vorhanden
                const owmIconUrl = day.icon
                    ? `https://openweathermap.org/img/wn/${day.icon}@4x.png`
                    : null;

                // Fallback Icon-Logik
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
        </section>
    );
}
