import { useQuery } from "@tanstack/react-query";
import { fetchSuppliers } from "src/services/api";
import { ApiResponse, SelectOption } from "src/services/types";

export function useSuppliers() {
    return useQuery<ApiResponse<SelectOption[]>, Error>({
        queryKey: ['suppliers'],
        queryFn: fetchSuppliers,
        staleTime: 1000 * 60 * 5,
        refetchOnWindowFocus: false,
    });
}