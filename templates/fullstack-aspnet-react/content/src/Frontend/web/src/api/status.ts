import { apiGet } from './client';

/** Mirrors ApplicationStatus in the backend's application layer. */
export interface ApplicationStatus {
  application: string;
  environment: string;
  databaseConnected: boolean;
}

export function fetchStatus(signal?: AbortSignal): Promise<ApplicationStatus> {
  return apiGet<ApplicationStatus>('/api/status', signal);
}
