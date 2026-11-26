import { TextEncoder, TextDecoder } from 'util';

Object.assign(globalThis, { TextEncoder, TextDecoder });

globalThis.fetch = jest.fn() as unknown as typeof fetch;

import '@testing-library/jest-dom';