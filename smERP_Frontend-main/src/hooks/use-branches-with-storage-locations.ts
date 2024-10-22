import { useQuery } from '@tanstack/react-query';
import { fetchBranchesWithStorageLocations } from 'src/services/api';
import { ApiResponse, BranchOption } from 'src/services/types';

export function useBranchesWithStorageLocations() {
  return useQuery<ApiResponse<BranchOption[]>, Error>({
    queryKey: ['branches-with-storage-locations'],
    queryFn: fetchBranchesWithStorageLocations,
    staleTime: 1000 * 60 * 5,
    refetchOnWindowFocus: false,
  });
}