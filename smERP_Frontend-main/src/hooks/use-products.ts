import { useQuery } from "@tanstack/react-query";
import { fetchProducts } from "src/services/api";
import { ApiResponse, ProductOption } from "src/services/types";

export function useProducts() {
    return useQuery<ApiResponse<ProductOption[]>, Error>({
        queryKey: ['products'],
        queryFn: fetchProducts,
        staleTime: 1000 * 60 * 5,
        refetchOnWindowFocus: false,
    });
}