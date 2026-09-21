import { render, screen } from '@testing-library/react';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { App } from './App';

/**
 * Example frontend test: `fetch` is stubbed, so the suite runs without a
 * backend. Delete this along with the status example.
 */
describe('App', () => {
  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it('renders the status returned by the API', async () => {
    stubFetch({
      ok: true,
      json: () =>
        Promise.resolve({
          application: '{{ProjectName}}.Api',
          environment: 'Development',
          databaseConnected: true,
        }),
    });

    render(<App />);

    expect(await screen.findByText('{{ProjectName}}.Api')).toBeInTheDocument();
    expect(screen.getByText('connected')).toBeInTheDocument();
  });

  it('reports a failure when the API is unreachable', async () => {
    stubFetch({ ok: false, status: 503, json: () => Promise.resolve({}) });

    render(<App />);

    expect(await screen.findByRole('alert')).toHaveTextContent('Could not reach the API');
  });
});

function stubFetch(response: Partial<Response> & { json: () => Promise<unknown> }) {
  vi.stubGlobal('fetch', vi.fn(() => Promise.resolve(response as unknown as Response)));
}
