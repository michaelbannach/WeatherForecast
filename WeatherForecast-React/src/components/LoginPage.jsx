import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const API_BASE = "http://localhost:5278";

export default function LoginPage() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleLogin = async (e) => {
        e.preventDefault();
        setMessage("");

        try {
            const resp = await fetch(`${API_BASE}/api/auth/login`, {
                method: "POST",
                credentials: "include",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, password }),
            });

            if (!resp.ok) {
                const msg = await resp.text();
                throw new Error(msg || "Login fehlgeschlagen");
            }

            // Erfolgreich eingeloggt → Weiterleitung zur Wetter-Startseite
            navigate("/");
        } catch (err) {
            setMessage(err.message);
        }
    };

    return (
        <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow w-full max-w-md">
            <h2 className="text-xl font-semibold mb-4">Login</h2>

            <form onSubmit={handleLogin} className="space-y-4">
                <div>
                    <label className="block text-sm font-medium text-slate-700">
                        E-Mail
                    </label>
                    <input
                        type="email"
                        required
                        className="mt-1 w-full border rounded-md px-3 py-2 bg-slate-50 text-sm"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium text-slate-700">
                        Passwort
                    </label>
                    <input
                        type="password"
                        required
                        className="mt-1 w-full border rounded-md px-3 py-2 bg-slate-50 text-sm"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>

                {message && <p className="text-red-600 text-sm">{message}</p>}

                <button
                    type="submit"
                    className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition"
                >
                    Einloggen
                </button>
            </form>
        </div>
    );
}
