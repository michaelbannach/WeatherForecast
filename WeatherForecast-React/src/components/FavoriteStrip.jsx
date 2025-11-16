import React from "react";

import FavoriteCard from "./FavoriteCard";

export default function FavoriteStrip({ favorites, onRemove, onSelect }) {
    if (!favorites || favorites.length === 0) return null;

    return (
        <section className="bg-white border border-slate-200 rounded-2xl shadow-sm p-4">
            <h2 className="text-sm font-semibold text-slate-700 mb-3">
                Favoriten
            </h2>

            <div className="grid grid-cols-5 gap-4 w-full">
                {favorites.map((f) => (
                    <FavoriteCard
                        key={f.id}
                        city={f.city}
                        country={f.country}
                        onDelete={() => onRemove(f.id)}
                        onSelect={onSelect}
                    />
                ))}
            </div>
        </section>
    );
}
