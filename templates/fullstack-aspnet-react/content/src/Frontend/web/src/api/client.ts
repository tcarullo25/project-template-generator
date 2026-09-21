import { appConfig } from '../config/env';

/** An API call that returned a non-2xx response. */
export class ApiError extends Error {
  constructor(
    readonly status: number,
    readonly path: string,
    message: string,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

/**
 * The single place that knows how to reach the backend: base URL, headers,
 * JSON handling and error shape. Feature modules build on this rather than
 * calling `fetch` directly.
 */
export async function apiGet<T>(path: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${appConfig.apiBaseUrl}${path}`, {
    method: 'GET',
    headers: { Accept: 'application/json' },
    signal,
  });

  if (!response.ok) {
    throw new ApiError(response.status, path, `GET ${path} failed with ${response.status}.`);
  }

  return (await response.json()) as T;
}
