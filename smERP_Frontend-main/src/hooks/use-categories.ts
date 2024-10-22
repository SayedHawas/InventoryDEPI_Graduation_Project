import { useQuery } from "@tanstack/react-query";
import { fetchParentCategories, fetchProductCategories } from "src/services/api";
import { ApiResponse, SelectOption } from "src/services/types";

export function useParentCategories() {
  return useQuery<ApiResponse<SelectOption[]>, Error>({
    queryKey: ['parentCategories'],
    queryFn: fetchParentCategories,
    staleTime: 1000 * 60 * 5,
    refetchOnWindowFocus: false,
  });
}

export function useProductCategories() {
  return useQuery<ApiResponse<SelectOption[]>, Error>({
    queryKey: ['productCategories'],
    queryFn: fetchProductCategories,
    staleTime: 1000 * 60 * 5,
    refetchOnWindowFocus: false,
  });
}