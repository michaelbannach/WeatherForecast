import React from "react";
import { X } from "lucide-react";

export default function FavoriteCard({ city, country, onDelete, onSelect }) {
    return (
        <div
            className="
        flex items-center justify-between
        px-4 py-3
        rounded-xl
        bg-white border border-slate-200 shadow-sm
        hover:shadow-md transition
        w-full
      "
        >
            
            <button
                type="button"
                onClick={() => onSelect?.(city, country)}
                className="text-left flex-1"
            >
        <span className="text-slate-800 font-medium truncate">
          {city}, {country}
        </span>
            </button>

           
            <button
                type="button"
                onClick={onDelete}
                className="p-1 rounded hover:bg-slate-100"
            >
                <X className="w-4 h-4 text-slate-500 hover:text-red-500" />
            </button>
        </div>
    );
}