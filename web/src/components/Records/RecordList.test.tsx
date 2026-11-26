import { fetchRecords } from './RecordList';



const mockRecords = [
  {
    id: '1',
    name: 'California',
    emoji: '🌴',
    walkingDistance: 5,
    totalPoints: 100,
    places: [
      {
        id: 'p1',
        name: 'Golden Gate',
        icon: '🌉',
        description: 'Famous bridge',
        visited: false,
        distance: 2,
        points: 50,
        stateId: '1',
      },
    ],
  },
];

describe('fetchRecords', () => {
  afterEach(() => {
    jest.restoreAllMocks();
  });

  it('returns an array of StateRecords on success', async () => {
    jest.spyOn(globalThis as any, 'fetch').mockResolvedValueOnce({
      ok: true,
      json: async () => mockRecords,
    });

    const result = await fetchRecords();
    expect(result).toHaveLength(1);
    expect(result[0].name).toBe('California');
  });

  it('throws when response is not ok', async () => {
    jest.spyOn(globalThis as any, 'fetch').mockResolvedValueOnce({
      ok: false,
      status: 500,
    });

    await expect(fetchRecords()).rejects.toThrow('Failed to load records (500)');
  });
});