import { useQuery } from "@tanstack/react-query";
import { Attribute } from "src/sections/product/attribute-selector";
import { fetchAttributes } from "src/services/api";
import { ApiResponse, AttributeSelectOption } from "src/services/types";

export function useAttributes() {
    return useQuery<ApiResponse<Attribute[]>, Error>({
      queryKey: ['attributes'],
      queryFn: fetchAttributes,
      staleTime: 1000 * 60 * 5,
      refetchOnWindowFocus: false,
    });
  }