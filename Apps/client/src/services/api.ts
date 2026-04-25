const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

export async function apiFetch<T>(
  endpoint: string,
  options?: RequestInit,
): Promise<T> {
  try {
    const res = await fetch(`${BASE_URL}${endpoint}`, {
      ...options,
      headers: {
        "Content-Type": "application/json",
        ...(options?.headers || {}),
      },
    });

    if (!res.ok) {
      const text = await res.text();
      throw new Error(`API Error ${res.status}: ${text}`);
    }

    return await res.json();
  } catch (error) {
    console.error("API Fetch Error:", error);
    throw error;
  }
}
