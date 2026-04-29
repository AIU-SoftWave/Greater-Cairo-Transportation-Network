"use client";
import { useEffect, useState } from "react";
import { MapView } from "@/components";
import { fetchNetworkTopology } from "@/services/network/networkTopology";
import { NetworkTopologyData } from "@/types";

export default function Home() {
  const [data, setData] = useState<NetworkTopologyData | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchNetworkTopology()
      .then(setData)
      .catch((err) => setError(err.message || "Failed to load data"));
  }, []);

  if (error) {
    return <div>Error: {error}</div>;
  }

  if (!data) {
    return <div>Loading...</div>;
  }

  return (
    <main className="h-screen w-full">
      <MapView nodes={data.nodes} edges={data.edges} />
    </main>
  );
}
