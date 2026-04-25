import MapLoader from "@/components/MapLoader";

export default function Home() {
  return (
    <main className="flex flex-col h-screen">
      <header className="p-4 bg-white shadow-sm border-b border-gray-200">
        <h1 className="text-2xl font-bold text-gray-800">
          Cairo Transportation Network
        </h1>
        <p className="text-sm text-gray-500 mt-1">
          Interactive map —{" "}
          <span className="inline-flex items-center gap-1">
            <span className="inline-block w-3 h-3 rounded-full bg-blue-500"></span>
            Node
          </span>{" "}
          <span className="inline-flex items-center gap-1 ml-2">
            <span className="inline-block w-3 h-3 rounded-full bg-red-500"></span>
            Critical node
          </span>
        </p>
      </header>
      <div className="flex-1 min-h-0">
        <MapLoader />
      </div>
    </main>
  );
}
