import { Search, Gauge, CloudSun, LogOut } from "lucide-react";
import {
    Sidebar,
    SidebarContent,
    SidebarHeader,
    SidebarMenu,
    SidebarMenuItem,
    SidebarMenuButton,
} from "@/components/ui/sidebar";
import { Link, useNavigate } from "react-router-dom";

const items = [
    { title: "Suche", icon: Search, path: "/" },
    { title: "Dashboard", icon: Gauge, path: "/dashboard" },
];

export function AppSidebar() {
    const navigate = useNavigate();

    const handleLogout = () => {
        
        localStorage.clear();
        sessionStorage.clear();
        document.cookie.split(";").forEach((c) => {
            document.cookie = c.replace(/^ +/, "")
                .replace(/=.*/, "=;expires=" + new Date().toUTCString() + ";path=/");
        });

        navigate("/login");
    };

    return (
        <Sidebar className="border-r border-slate-800 bg-slate-950/95 text-slate-100 flex flex-col">
            <SidebarHeader className="px-4 pt-6 pb-4">
                <div className="flex items-center gap-3">
                    <div className="w-9 h-9 rounded-2xl bg-sky-500/15 border border-sky-400/40 flex items-center justify-center">
                        <CloudSun className="w-5 h-5 text-sky-300" />
                    </div>
                    <div className="leading-tight">
                        <div className="font-semibold tracking-tight">WetterApp</div>
                    </div>
                </div>
            </SidebarHeader>

            {/* Navigation */}
            <SidebarContent className="px-2 flex-grow">
                <SidebarMenu>
                    {items.map((item) => (
                        <SidebarMenuItem key={item.title}>
                            <SidebarMenuButton asChild>
                                <Link
                                    to={item.path}
                                    className="w-full flex items-center gap-2 px-3 py-2 rounded-xl text-sm text-slate-200 hover:bg-slate-800/80 hover:text-sky-300 transition"
                                >
                                    <item.icon className="w-4 h-4" />
                                    <span>{item.title}</span>
                                </Link>
                            </SidebarMenuButton>
                        </SidebarMenuItem>
                    ))}
                </SidebarMenu>
            </SidebarContent>

            {/* LOGOUT BUTTON  */}
            <div className="px-4 pb-4 mt-auto">
                <button
                    onClick={handleLogout}
                    className="w-full flex items-center gap-2 px-3 py-2 rounded-xl text-sm bg-slate-800/50 hover:bg-slate-800/80 text-red-300 hover:text-red-400 transition"
                >
                    <LogOut className="w-4 h-4" />
                    <span>Logout</span>
                </button>
            </div>
        </Sidebar>
    );
}
