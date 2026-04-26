import { MapView } from "@/components";
import { fetchNetworkTopology } from "@/services/network/networkTopology";

export default async function Home() {
  const data = await fetchNetworkTopology();

  return (
    <main className="h-screen w-full">
      <MapView nodes={data.nodes} edges={data.edges} />
    </main>
  );
}
