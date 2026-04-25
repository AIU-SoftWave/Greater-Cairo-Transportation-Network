"use client";

import dynamic from "next/dynamic";

const CairoMap = dynamic(() => import("./CairoMap"), { ssr: false });

export default function MapLoader() {
  return <CairoMap />;
}
