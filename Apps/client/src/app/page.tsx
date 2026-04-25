import { fetchNetworkTopology } from "@/services/networkTopology";

export default  function Home() {
  const data =  fetchNetworkTopology();  
  console.log(data);
  
  return (
    <main className="flex h-screen items-center justify-center">
      <h1 className="text-2xl font-bold">Cairo Transportation System</h1>
    </main>
  );
}
