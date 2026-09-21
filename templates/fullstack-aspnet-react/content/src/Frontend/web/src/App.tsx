import { useEffect, useState } from 'react';
import { fetchStatus, type ApplicationStatus } from './api/status';

type LoadState =
  | { kind: 'loading' }
  | { kind: 'loaded'; status: ApplicationStatus }
  | { kind: 'failed'; message: string };

/**
 * The application shell. It calls the backend's /api/status endpoint on mount,
 * which is the template's proof that React -> ASP.NET Core -> PostgreSQL is
 * wired correctly. Replace it with your own UI.
 */
export function App() {
  const [state, setState] = useState<LoadState>({ kind: 'loading' });

  useEffect(() => {
    const controller = new AbortController();

    fetchStatus(controller.signal)
      .then((status) => setState({ kind: 'loaded', status }))
      .catch((error: unknown) => {
        if (controller.signal.aborted) {
          return;
        }

        setState({
          kind: 'failed',
          message: error instanceof Error ? error.message : 'Unknown error.',
        });
      });

    return () => controller.abort();
  }, []);

  return (
    <main>
      <h1>{{ProjectName}}</h1>
      <p>Generated project shell. Backend status:</p>
      {state.kind === 'loading' && <p>Checking backend...</p>}
      {state.kind === 'failed' && (
        <p className="error" role="alert">
          Could not reach the API: {state.message}
        </p>
      )}
      {state.kind === 'loaded' && (
        <dl>
          <dt>Application</dt>
          <dd>{state.status.application}</dd>
          <dt>Environment</dt>
          <dd>{state.status.environment}</dd>
          <dt>Database</dt>
          <dd>{state.status.databaseConnected ? 'connected' : 'not connected'}</dd>
        </dl>
      )}
    </main>
  );
}
