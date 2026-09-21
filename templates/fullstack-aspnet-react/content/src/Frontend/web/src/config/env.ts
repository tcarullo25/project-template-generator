/**
 * Every environment-provided value the frontend reads, in one place.
 *
 * Vite inlines `import.meta.env.VITE_*` at build time, so these are build-time
 * values, not runtime ones — a container image built for one environment cannot
 * be re-pointed at another without rebuilding.
 */
export interface AppConfig {
  /** Base URL for API requests. Empty means "the origin serving this page". */
  readonly apiBaseUrl: string;
}

export const appConfig: AppConfig = {
  apiBaseUrl: (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/+$/, ''),
};
