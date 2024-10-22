import { useQuery } from "@tanstack/react-query";
import { fetchRoles } from "src/services/api";
import { ApiResponse } from "src/services/types";

export function useRoles() {
    return useQuery<ApiResponse<string[]>, Error>({
      queryKey: ['roles'],
      queryFn: fetchRoles,
      staleTime: 1000 * 60 * 5,
      refetchOnWindowFocus: false,
    });
  }