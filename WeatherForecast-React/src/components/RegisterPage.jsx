import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const API_BASE = "http://localhost:5278";

export default function RegisterPage() {
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");

    const [isSuperUser, setIsSuperUser] = useState(false);
    const [message, setMessage] = useState("");

    const handleRegister = async (e) => {
        e.preventDefault();
        setMessage("");

        const role = isSuperUser ? "SuperUser" : "NormalUser";

        try {
            const resp = await fetch(`${API_BASE}/api/auth/register`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    Email: email,
                    Password: password,
                    FirstName: firstName,
                    LastName: lastName,
                    Role: role
                }),
            });

            if (!resp.ok) {
                const msg = await resp.text();
                throw new Error(msg || "Registrierung fehlgeschlagen");
            }

            setMessage("Benutzer erfolgreich registriert!");

            setTimeout(() => navigate("/login"), 1000);

        } catch (err) {
            setMessage(err.message);
        }
    };

    return (
        <div className="bg-white border border-slate-200 rounded-2xl shadow w-full max-w-md p-6">
            <h2 className="text-xl font-semibold mb-4">Registrieren</h2>

            <form onSubmit={handleRegister} className="space-y-4">

                <div>
                    <label className="block text-sm font-medium text-slate-700">
                        Vorname
                    </label>
                    <input
                        type="text"
                        required
                        className="mt-1 w-full border rounded-md px-3 py-2 bg-slate-50 text-sm"
                        value={firstName}
                        onChange={(e) => setFirstName(e.target.value)}
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium text-slate-700">
                        Nachname
                    </label>
                    <input
                        type="text"
                        required
                        className="mt-1 w-full border rounded-md px-3 py-2 bg-slate-50 text-sm"
                        value={lastName}
                        onChange={(e) => setLastName(e.target.value)}
                    />
                </div>

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

                <label className="flex items-center gap-2 text-sm text-slate-700">
                    <input
                        type="checkbox"
                        checked={isSuperUser}
                        onChange={(e) => setIsSuperUser(e.target.checked)}
                        className="rounded border-slate-300"
                    />
                    Registrieren als SuperUser
                </label>

                {message && <p className="text-sm mt-1 text-emerald-600">{message}</p>}

                <button
                    type="submit"
                    className="mt-2 w-full bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium px-4 py-2 rounded-md"
                >
                    Registrieren
                </button>

            </form>
        </div>
    );
}
