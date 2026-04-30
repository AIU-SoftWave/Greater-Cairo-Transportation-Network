import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Greater Cairo Transportation System",
  description:
    "An interactive visualization of the transportation system in Greater Cairo, Egypt. using diffrent algorithms for pathfinding and route optimization.",
  authors: [{ name: "Ahmed Saleh", url: "https://github.com/Aboosalh" }],
  keywords: [
    "Greater Cairo",
    "Transportation System",
    "Pathfinding",
    "Route Optimization",
    "Interactive Visualization",
    "Algorithms",
    "Shortest Path",
    "Dijkstra",
    "A*",
    "Genetic Algorithm",
    "Simulated Annealing",
    "Tabu Search",
  ],
  
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="min-h-full flex flex-col">{children}</body>
    </html>
  );
}
