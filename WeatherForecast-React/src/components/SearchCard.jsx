import React from "react";
import { MapPin, Search } from "lucide-react";

export function SearchCard({
                               city,
                               setCity,
                               country,
                               setCountry,
                               loading,
                               onSubmit,
                               error,
                           }) {
    return (
        <section className="bg-white border border-slate-200 rounded-xl shadow-sm p-5 space-y-4">
            <header>
                <h3 className="text-lg font-semibold">Wetter abfragen</h3>
               
            </header>

            <form
                onSubmit={onSubmit}
                className="flex flex-col md:flex-row gap-3 items-end"
            >
                {/* Stadt */}
                <div className="flex-1">
                    <label className="block text-xs font-medium text-slate-600 mb-1">
                        Stadt
                    </label>
                    <div className="flex items-center gap-2 border border-slate-300 rounded-md px-3 py-2 bg-slate-50">
                        <MapPin className="w-4 h-4 text-slate-400" />
                        <input
                            type="text"
                            value={city}
                            onChange={(e) => setCity(e.target.value)}
                            placeholder="z. B. Berlin"
                            className="w-full bg-transparent outline-none text-sm"
                        />
                    </div>
                </div>

                {/* Land */}
                <div className="w-full md:w-32">
                    <label className="block text-xs font-medium text-slate-600 mb-1">
                        Land (Code)
                    </label>
                    <input
                        type="text"
                        value={country}
                        onChange={(e) => setCountry(e.target.value.toUpperCase())}
                        placeholder="DE"
                        maxLength={2}
                        className="w-full border border-slate-300 rounded-md px-3 py-2 bg-slate-50 text-sm uppercase outline-none"
                    />
                </div>

                {/* Button */}
                <button
                    type="submit"
                    disabled={loading}
                    className="inline-flex items-center gap-2 px-4 py-2 rounded-md bg-slate-900 text-white text-sm font-medium hover:bg-slate-800 disabled:opacity-60"
                >
                    <Search className="w-4 h-4" />
                    {loading ? "Lade..." : "Suchen"}
                </button>
            </form>

            {error && (
                <p className="text-xs text-rose-500 font-medium mt-1">{error}</p>
            )}
        </section>
    );
}
