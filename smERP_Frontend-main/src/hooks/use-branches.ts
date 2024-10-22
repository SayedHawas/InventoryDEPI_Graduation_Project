import { useQuery } from '@tanstack/react-query';
import { fetchBranches } from 'src/services/api';
import { ApiResponse, SelectOption } from 'src/services/types';

export function useBranches() {
  return useQuery<ApiResponse<SelectOption[]>, Error>({
    queryKey: ['branches'],
    queryFn: fetchBranches,
    staleTime: 1000 * 60 * 5,
    refetchOnWindowFocus: false,
  });
}