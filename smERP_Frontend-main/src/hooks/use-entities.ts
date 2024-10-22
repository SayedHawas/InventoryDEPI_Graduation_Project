import { useState, useEffect, useCallback } from 'react';
import { fetchEntities, useAuthenticatedFetch } from 'src/services/api';
import { PaginationParameters, ApiPaginatedResponse } from 'src/services/types';

export const useEntities = <T>(
  endpoint: string,
  initialParams: PaginationParameters,
  transformEntity?: (entity: any) => T
) => {
  const [entities, setEntities] = useState<T[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [params, setParams] = useState<PaginationParameters>(initialParams);

  const authenticatedFetch = useAuthenticatedFetch();
  
  const getEntities = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const apiResponse: ApiPaginatedResponse<T[]> = await fetchEntities<T[]>(authenticatedFetch,endpoint, params);
      if (apiResponse.isSuccess && apiResponse.value.data) {
        setEntities(transformEntity ? apiResponse.value.data.map(transformEntity) : apiResponse.value.data);
        setTotalCount(apiResponse.value.totalCount);
      } else {
        throw new Error(apiResponse.message || `Failed to fetch ${endpoint}`);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  }, [endpoint, params, transformEntity]);

  useEffect(() => {
    getEntities();
  }, [getEntities]);

  const updateParams = useCallback((newParams: Partial<PaginationParameters>) => {
    setParams(prevParams => ({ ...prevParams, ...newParams }));
  }, []);

  const refetch = useCallback(() => {
    getEntities();
  }, [getEntities]);

  return { entities, loading, error, totalCount, updateParams, refetch };
};