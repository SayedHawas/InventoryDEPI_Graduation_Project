import { useMutation, useQuery, UseQueryOptions } from '@tanstack/react-query';
import { jwtDecode } from 'jwt-decode';
import { ApiPaginatedResponse, ApiResponse, AttributeSelectOption, BranchOption, PaginationParameters, ProductOption, SelectOption } from './types';
import { BranchFormData } from 'src/sections/branch/branch-form';
import { BrandFormData } from 'src/sections/brand/brand-form';
import { CategoryFormData } from 'src/sections/category/category-form';
import { UserFormData } from 'src/sections/user/add-new-user';
import { AttributeFormData } from 'src/sections/attribute/attribute-form';
import { ProductFormData } from 'src/sections/product/product-form';
import { ProductInstanceFormData } from 'src/sections/product/product-instance-form';
import { Attribute } from 'src/sections/product/attribute-selector';
import { SupplierFormData } from 'src/sections/supplier/supplier-form';
import { ProcurementFormData } from 'src/sections/transaction/procurement/procurement-form';
import { StorageLocationFormData } from 'src/sections/branch/storage-location-form';
import { PaymentFormData } from 'src/sections/transaction/procurement/payment-form';
import { TransactionProductFormData } from 'src/sections/transaction/procurement/product-form';

// Define the base URL for your API
const API_BASE_URL = 'https://smerp.runasp.net';

// Types for our auth responses
interface LoginResponse {
  token: string;
  refreshTokenExpirationDate: Date;
}

interface RefreshResponse {
  jwt: string;
}

interface User {
  id: string;
  firstName:string;
  lastName:string;
  email: string;
  roles: string[];
  branch: string;
  unique_name: string;
}

interface JwtPayload {
  id: string;
  email: string;
  userName: string;
  firstName: string;
  lastName: string;
  role: string[];
  exp: number;
}

// Helper function to handle API calls
const apiCall = async <T>(
  endpoint: string,
  method: string,
  body?: object,
  token?: string
): Promise<T> => {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method,
    headers,
    body: body ? JSON.stringify(body) : undefined,
    credentials: 'include',
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  return response.json();
};

// Function to handle login
const loginUser = async (credentials: { email: string; password: string }) => {
  const data = await apiCall<ApiResponse<LoginResponse>>('/Auth/login', 'POST', credentials);
  localStorage.setItem('accessToken', data.value.token);
  const user: User = jwtDecode<User>(data.value.token)
  localStorage.setItem('user', JSON.stringify(user));
  return user;
};

// Function to handle logout
const logoutUser = async () => {
  localStorage.removeItem('accessToken');
  localStorage.removeItem('user');
  await apiCall('/Auth/revokeToken', 'GET');
};

// Function to refresh the access token
const refreshAccessToken = async () => {
  const data = await apiCall<RefreshResponse>('/Auth/refreshToken', 'GET');
  localStorage.setItem('accessToken', data.jwt);
  const user: User = jwtDecode<User>(data.jwt)
  localStorage.setItem('user', JSON.stringify(user));
  return data;
};

export const useLogin = () => {
  const mutation = useMutation({
    mutationFn: loginUser,
  });

  return {
    login: mutation.mutate,
    isLoading: mutation.status === 'pending',
    ...mutation,
  };
};

export const useLogout = () =>
  useMutation({ mutationFn: logoutUser });


// Custom hook for token refresh mutation
export const useRefreshToken = () =>
  useMutation({ mutationFn: refreshAccessToken });


export const isTokenExpired = (token?: string): boolean => {
  // Use the provided token or retrieve from local storage
  const tokenToCheck = token || localStorage.getItem('accessToken');

  if (!tokenToCheck) {
    // No token available to check
    return true;
  }

  try {
    // Decode the token
    const decodedToken = jwtDecode<JwtPayload>(tokenToCheck);
    // Check if the token is expired
    return decodedToken.exp * 1000 < Date.now();
  } catch {
    // If decoding fails, assume the token is expired
    return true;
  }
};

export const useAuthenticatedFetch = () => {
  const refreshTokenMutation = useRefreshToken();

  return async (input: RequestInfo, init?: RequestInit) => {
    let accessToken = localStorage.getItem('accessToken') || '';

    if (!accessToken || isTokenExpired(accessToken)) {
      try {
        const refreshResult = await refreshTokenMutation.mutateAsync();
        accessToken = refreshResult.jwt;
      } catch (error) {
        throw new Error('Unable to refresh token');
      }
    }

    const headers = new Headers(init?.headers);
    headers.set('Authorization', `Bearer ${accessToken}`);

    const response = await fetch(input, { ...init, headers });

    if (response.status === 401) {
      throw new Error('Unauthorized');
    }

    return response;
  };
};

// Custom hook for authenticated queries
export const useAuthenticatedQuery = <TData>(
  key: string[],
  queryFn: () => Promise<TData>,
  options?: Omit<UseQueryOptions<TData, Error>, 'queryKey' | 'queryFn'>
) => {
  const authenticatedFetch = useAuthenticatedFetch();

  return useQuery<TData, Error>({
    queryKey: key,
    queryFn: async () => {
      try {
        const response = await authenticatedFetch(queryFn.toString());
        return await response.json();
      } catch (error) {
        if (error instanceof Error && error.message === 'Unauthorized') {
          localStorage.removeItem('accessToken');
          localStorage.removeItem('user');
        }
        throw error;
      }
    },
    retry: (failureCount: number, error: Error) => {
      if (error instanceof Error && error.message === 'Unauthorized') {
        return false;
      }
      return failureCount < 3;
    },
    ...options,
  });
};

export const API_BASE_URL1 = 'https://smerp.runasp.net/';

async function handleResponse(response: Response) {
  if (!response.ok) {
    const errorResponse = await response.json();
    const errorMessages = errorResponse.errorMessages || ["An unknown error occurred."];
    throw new Error(errorMessages.join(', '));
  }
  return response.json();
}

export const apiService = {
  users: {
    create: async (data: UserFormData) => {
      const response = await fetch(`${API_BASE_URL1}auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    update: async (userId: string, data: Partial<UserFormData>) => {
      const response = await fetch(`${API_BASE_URL1}auth/users/${userId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
  },
  brands: {
    create: async (data: BrandFormData) => {
      const response = await fetch(`${API_BASE_URL1}brands`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    update: async (brandId: string, data: Partial<BrandFormData>) => {
      const response = await fetch(`${API_BASE_URL1}brands`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ...data, brandId }),
      });
      return handleResponse(response);
    },
  },
  branches: {
    create: async (data: BranchFormData) => {
      const response = await fetch(`${API_BASE_URL1}branches`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    update: async (branchId: string, data: Partial<BranchFormData>) => {
      const response = await fetch(`${API_BASE_URL1}branches`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ...data, branchId }),
      });
      return handleResponse(response);
    },
  },
  categories: {
    create: async (data: CategoryFormData) => {
      const response = await fetch(`${API_BASE_URL1}categories`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    update: async (categoryId: string, data: Partial<CategoryFormData>) => {
      const response = await fetch(`${API_BASE_URL1}categories`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ...data, categoryId }),
      });
      return handleResponse(response);
    },
  },
  attributes: {
    create: async (data: AttributeFormData) => {
      const response = await fetch(`${API_BASE_URL1}attributes`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    update: async (attributeId: string, data: any) => {
      const response = await fetch(`${API_BASE_URL1}attributes`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ...data, attributeId }),
      });
      return handleResponse(response);
    },
  },
  products: {
    create: async (data: ProductFormData) => {
      const response = await fetch(`${API_BASE_URL1}products`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    update: async (productId: string, data: Partial<ProductFormData>) => {
      const response = await fetch(`${API_BASE_URL1}products`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ...data, productId }),
      });
      return handleResponse(response);
    },
  },
  productInstances: {
    create: async (productId: string ,data: ProductInstanceFormData) => {
      const response = await fetch(`${API_BASE_URL1}products/${productId}/instances`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ...data, productId }),
      });
      return handleResponse(response);
    },
    update: async (productId: string, productInstanceId: string, data: Partial<ProductInstanceFormData>) => {
      const response = await fetch(`${API_BASE_URL1}products/${productId}/instances/${productInstanceId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
  },
  suppliers: {
    create: async (data: SupplierFormData) => {
      const response = await fetch(`${API_BASE_URL1}suppliers/`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    update: async (supplierId: string, data: Partial<SupplierFormData>) => {
      const response = await fetch(`${API_BASE_URL1}suppliers/${supplierId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    getById: async (supplierId: number) =>{
      const response = await fetch(`${API_BASE_URL1}suppliers/${supplierId}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
      });
      return handleResponse(response);
    }
  },
  procurements: {
    create: async (data: ProcurementFormData) => {
      const response = await fetch(`${API_BASE_URL1}procurementTransactions/`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    update: async (procurementTransactionId: string, data: Partial<ProcurementFormData>) => {
      const response = await fetch(`${API_BASE_URL1}procurementTransactions/${procurementTransactionId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    getById: async (procurementTransactionId: number) =>{
      const response = await fetch(`${API_BASE_URL1}procurementTransactions/${procurementTransactionId}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
      });
      return handleResponse(response);
    },
    payments: {
      get: async (transactionId: string, paymentId: string) => {
        const response = await fetch(`${API_BASE_URL1}procurementTransactions/${transactionId}/payments/${paymentId}`, {
          method: 'GET',
          headers: { 'Content-Type': 'application/json' },
        });
        return handleResponse(response);
      },
      create: async (data: PaymentFormData) => {
        const response = await fetch(`${API_BASE_URL1}procurementTransactions/payments`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data),
        });
        return handleResponse(response);
      },
      update: async (data: Partial<PaymentFormData>) => {
        const response = await fetch(`${API_BASE_URL1}procurementTransactions/payments`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data),
        });
        return handleResponse(response);
      },
      delete: async (transactionId: string, paymentId: string) => {
        const response = await fetch(`${API_BASE_URL1}procurementTransactions/${transactionId}/payments/${paymentId}`, {
          method: 'DELETE',
          headers: { 'Content-Type': 'application/json' },
        });
        return handleResponse(response);
      }
    },
    products: {
      get: async (transactionId: string, productId: string) => {
        const response = await fetch(`${API_BASE_URL1}procurementTransactions/${transactionId}/products/${productId}`, {
          method: 'GET',
          headers: { 'Content-Type': 'application/json' },
        });
        return handleResponse(response);
      },
      create: async (data: TransactionProductFormData) => {
        const response = await fetch(`${API_BASE_URL1}procurementTransactions/products`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data),
        });
        return handleResponse(response);
      },
      update: async (data: Partial<TransactionProductFormData>) => {
        const response = await fetch(`${API_BASE_URL1}procurementTransactions/products`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data),
        });
        return handleResponse(response);
      },
      delete: async (transactionId: string, productId: string) => {
        const response = await fetch(`${API_BASE_URL1}procurementTransactions/${transactionId}/products/${productId}`, {
          method: 'DELETE',
          headers: { 'Content-Type': 'application/json' },
        });
        return handleResponse(response);
      }
    }
  },
  storageLocations: {
    create: async (branchId: string,data: StorageLocationFormData) => {
      const response = await fetch(`${API_BASE_URL1}branches/${branchId}/storage-locations`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    update: async (branchId: string, storageLocationId: string, data: Partial<StorageLocationFormData>) => {
      data.branchId = branchId;
      data.storageLocationsId = storageLocationId;
      const response = await fetch(`${API_BASE_URL1}branches/${branchId}/storage-locations/${storageLocationId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      return handleResponse(response);
    },
    getById: async (branchId: string, storageLocationId: string) =>{
      const response = await fetch(`${API_BASE_URL1}branches/${branchId}/storage-locations/${storageLocationId}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
      });
      return handleResponse(response);
    }
  }
};

// export const fetchEntities = async <T>(
//   endpoint: string,
//   params: PaginationParameters
// ): Promise<ApiPaginatedResponse<T>> => {
//   const queryString = new URLSearchParams(
//     Object.entries(params).reduce((acc, [key, value]) => {
//       if (value != null && value !== '') {
//         acc[key] = String(value);
//       }
//       return acc;
//     }, {} as Record<string, string>)
//   ).toString();

//   const response = await fetch(`${API_BASE_URL1}${endpoint}${queryString ? `?${queryString}` : ''}`);

//   if (!response.ok) {
//     throw new Error(`Failed to fetch ${endpoint}`);
//   }

//   return response.json();
// };

// export const fetchEntities = async <T>(
//   endpoint: string,
//   params: PaginationParameters,
// ): Promise<ApiPaginatedResponse<T>> => {
//   const queryString = new URLSearchParams(
//     Object.entries(params).reduce((acc, [key, value]) => {
//       if (value != null && value !== '') {
//         acc[key] = String(value);
//       }
//       return acc;
//     }, {} as Record<string, string>)
//   ).toString();

//   const accessToken = localStorage.getItem('accessToken');

//   const response = await fetch(`${API_BASE_URL1}${endpoint}${queryString ? `?${queryString}` : ''}`, {
//     method: 'GET',
//     headers: {
//       'Authorization': `Bearer ${accessToken}`,
//     },
//   });

//   if (!response.ok) {
//     throw new Error(`Failed to fetch ${endpoint}`);
//   }

//   return response.json();
// };

export const fetchEntities1 = async <T>(
  authenticatedFetch: (input: RequestInfo, init?: RequestInit) => Promise<Response>,
  endpoint: string,
  params: PaginationParameters,
): Promise<ApiPaginatedResponse<T>> => {
  const queryString = new URLSearchParams(
    Object.entries(params).reduce((acc, [key, value]) => {
      if (value != null && value !== '') {
        acc[key] = String(value);
      }
      return acc;
    }, {} as Record<string, string>)
  ).toString();

  const response = await authenticatedFetch(`${API_BASE_URL1}${endpoint}${queryString ? `?${queryString}` : ''}`, {
    method: 'GET',
  });

  if (!response.ok) {
    throw new Error(`Failed to fetch ${endpoint}`);
  }

  return response.json();
};

export const fetchEntities = async <T>(
  authenticatedFetch: (input: RequestInfo, init?: RequestInit) => Promise<Response>,
  endpoint: string,
  params: PaginationParameters,
): Promise<ApiPaginatedResponse<T>> => {
  const queryParameters: Record<string, string> = {};

  Object.entries(params).forEach(([key, value]) => {
    if (value != null && value !== '') {
      if (key !== 'FilterBy') {
        queryParameters[key] = String(value);
      }
    }
  });

  if (params.FilterBy) {
    params.FilterBy.forEach(filter => {
      if (filter.filterBy && filter.value) {
        queryParameters[filter.filterBy] = filter.value;
      }
    });
  }


  const queryString = new URLSearchParams(queryParameters).toString();

  const url = `${API_BASE_URL1}${endpoint}?${queryString}`;
  
  const response = await authenticatedFetch(url);

  if (!response.ok) {
    throw new Error(`Error fetching entities: ${response.statusText}`);
  }

  return await response.json();
};


export const fetchBranches = async (): Promise<ApiResponse<SelectOption[]>> => {
  const response = await fetch(`${API_BASE_URL1}branches/list`);

  if (!response.ok) {
    throw new Error('Failed to fetch branches');
  }

  const result: ApiResponse<SelectOption[]> = await response.json();
  return result;
};

export const fetchBranchesWithStorageLocations = async (): Promise<ApiResponse<BranchOption[]>> => {
  const response = await fetch(`${API_BASE_URL1}branches/list-with-storage-locations`);

  if (!response.ok) {
    throw new Error('Failed to fetch branches');
  }

  const result: ApiResponse<BranchOption[]> = await response.json();
  return result;
};

export const fetchAttributes = async (): Promise<ApiResponse<Attribute[]>> => {
  const response = await fetch(`${API_BASE_URL1}attributes/list`);

  if (!response.ok) {
    throw new Error('Failed to fetch attributes');
  }

  const result: ApiResponse<Attribute[]> = await response.json();
  return result;
};

export const fetchBrands = async (): Promise<ApiResponse<SelectOption[]>> => {
  const response = await fetch(`${API_BASE_URL1}brands/list`);

  if (!response.ok) {
    throw new Error('Failed to fetch brands');
  }

  const result: ApiResponse<SelectOption[]> = await response.json();
  return result;
};

export const fetchRoles = async (): Promise<ApiResponse<string[]>> => {
  const response = await fetch(`${API_BASE_URL1}auth/roles`);

  if (!response.ok) {
    throw new Error('Failed to fetch branches');
  }

  const result: ApiResponse<string[]> = await response.json();
  return result;
};

export const fetchParentCategories = async (): Promise<ApiResponse<SelectOption[]>> => {
  const response = await fetch(`${API_BASE_URL1}categories/parent`);

  if (!response.ok) {
    throw new Error('Failed to fetch categories');
  }

  const result: ApiResponse<SelectOption[]> = await response.json();
  return result;
};

export const fetchProductCategories = async (): Promise<ApiResponse<SelectOption[]>> => {
  const response = await fetch(`${API_BASE_URL1}categories/product`);

  if (!response.ok) {
    throw new Error('Failed to fetch categories');
  }

  const result: ApiResponse<SelectOption[]> = await response.json();
  return result;
};

export const fetchSuppliers = async (): Promise<ApiResponse<SelectOption[]>> => {
  const response = await fetch(`${API_BASE_URL1}suppliers/list`);

  if (!response.ok) {
    throw new Error('Failed to fetch suppliers');
  }

  const result: ApiResponse<SelectOption[]> = await response.json();
  return result;
};

export const fetchProducts = async (): Promise<ApiResponse<ProductOption[]>> => {
  const response = await fetch(`${API_BASE_URL1}products/list`);

  if (!response.ok) {
    throw new Error('Failed to fetch products');
  }

  const result: ApiResponse<ProductOption[]> = await response.json();
  return result;
};

export { apiCall };