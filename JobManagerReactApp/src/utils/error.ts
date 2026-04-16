import axios from 'axios';

function getApiResponseMessage(data: unknown): string | null {
    if (!data || typeof data !== 'object') return null;
    if (!('message' in data)) return null;

    const message = (data as { message: unknown }).message;
    return typeof message === 'string' ? message : null;
}

export function parseErrorMessage(err: unknown): string {
    if (axios.isAxiosError(err)) {
        return getApiResponseMessage(err.response?.data) ?? err.message ?? 'Request failed';
    }

    if (err instanceof Error) {
        return err.message;
    }

    return 'Unexpected error';
}
