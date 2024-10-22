import { useQuery } from "@tanstack/react-query";
import { fetchBrands } from "src/services/api";
import { ApiResponse, SelectOption } from "src/services/types";

export function useBrands() {
  return useQuery<ApiResponse<SelectOption[]>, Error>({
    queryKey: ['brands'],
    queryFn: fetchBrands,
    staleTime: 1000 * 60 * 5,
    refetchOnWindowFocus: false,
  });
}