import React from "react";
import { Sun, Sunset } from "lucide-react";

export default function SunriseSunsetCards({ weather }) {
    if (!weather) return null;

    // Unix → HH:MM
    const formatTime = (unix) => {
        return new Date(unix * 1000).toLocaleTimeString("de-DE", {
            hour: "2-digit",
            minute: "2-digit",
        });
    };

    return (
        <div className="grid grid-cols-2 gap-4 mt-4">
            <div className="bg-white rounded-xl p-5 shadow-sm flex flex-col items-center">
                <Sun className="w-10 h-10 text-yellow-500 mb-2" />
                <p className="text-xs text-slate-500">Sonnenaufgang</p>
                <p className="text-xl font-semibold mt-1">
                    {formatTime(weather.sunrise)}
                </p>
            </div>

           
            <div className="bg-white rounded-xl p-5 shadow-sm flex flex-col items-center">
                <Sunset className="w-10 h-10 text-orange-500 mb-2" />
                <p className="text-xs text-slate-500">Sonnenuntergang</p>
                <p className="text-xl font-semibold mt-1">
                    {formatTime(weather.sunset)}
                </p>
            </div>
        </div>
    );
}
