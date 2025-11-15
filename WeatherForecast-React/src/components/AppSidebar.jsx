// src/AppSidebar.jsx
import { Search, Gauge, CloudSun } from "lucide-react";
import {
    Sidebar,
    SidebarContent,
    SidebarHeader,
    SidebarMenu,
    SidebarMenuItem,
    SidebarMenuButton,
} from "@/components/ui/sidebar";

const items = [
    { title: "Suche", icon: Search },
    { title: "Dashboard", icon: Gauge },
];

export function AppSidebar() {
    return (
        <Sidebar className="border-r border-slate-800 bg-slate-950/95 text-slate-100">
            <SidebarHeader className="px-4 pt-6 pb-4">
                <div className="flex items-center gap-3">
                    <div className="w-9 h-9 rounded-2xl bg-sky-500/15 border border-sky-400/40 flex items-center justify-center">
                        <CloudSun className="w-5 h-5 text-sky-300" />
                    </div>
                    <div className="leading-tight">
                        <div className="font-semibold tracking-tight">WetterApp</div>
                        <div className="text-[10px] text-slate-400">
                            Schnell & übersichtlich.
                        </div>
                    </div>
                </div>
            </SidebarHeader>

            <SidebarContent className="px-2">
                <SidebarMenu>
                    {items.map((item) => (
                        <SidebarMenuItem key={item.title}>
                            <SidebarMenuButton asChild>
                                <button className="w-full flex items-center gap-2 px-3 py-2 rounded-xl text-sm text-slate-200 hover:bg-slate-800/80 hover:text-sky-300 transition">
                                    <item.icon className="w-4 h-4" />
                                    <span>{item.title}</span>
                                </button>
                            </SidebarMenuButton>
                        </SidebarMenuItem>
                    ))}
                </SidebarMenu>
            </SidebarContent>
        </Sidebar>
    );
}
