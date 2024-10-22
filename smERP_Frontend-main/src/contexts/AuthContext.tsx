import React, { createContext, useState, useContext, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { UseQueryOptions } from '@tanstack/react-query';
import { jwtDecode } from 'jwt-decode';
import { Notification } from '../services/types';
import { isTokenExpired, useAuthenticatedFetch, useAuthenticatedQuery, useLogin, useLogout, useRefreshToken } from '../services/api';
import { signalRService } from 'src/services/signalR';

interface User {
  id: string;
  email: string;
  firstName:string;
  lastName:string;
  unique_name: string;
  branch: string;
  roles: string[]
}

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  authenticatedFetch: (input: RequestInfo, init?: RequestInit) => Promise<Response>;
  authenticatedQuery: <TData>(
    key: string[],
    queryFn: () => Promise<TData>,
    options?: Omit<UseQueryOptions<TData, Error>, 'queryKey' | 'queryFn'>
  ) => ReturnType<typeof useAuthenticatedQuery<TData>>;
  notifications: Notification[];
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const loginMutation = useLogin();
  const logoutMutation = useLogout();
  const refreshTokenMutation = useRefreshToken();
  const authenticatedFetch = useAuthenticatedFetch();

  useEffect(() => {
    const initAuth = async () => {
      try {
        const token = localStorage.getItem('accessToken');
        if (token && !isTokenExpired(token)) {
          const userFromToken: User = jwtDecode<User>(token);
          setUser(userFromToken);
          await signalRService.startConnection(token);
        } else if (token) {
          await refreshTokenMutation.mutateAsync();
        }
      } catch (error) {
        console.error("Error initializing auth:", error);
        setUser(null);
      } finally {
        setIsLoading(false);
      }
    };
    initAuth();

    return () => {
      signalRService.stopConnection();
    };
  }, []);

  useEffect(() => {
    const handleNotification = (newNotifications: Notification | Notification[]) => {
      setNotifications(prev => [
        ...(Array.isArray(newNotifications) ? newNotifications : [newNotifications]),
        ...prev
      ]);
    };

    if (user) {
      signalRService.onNotification(handleNotification);
    }

    return () => {
      if (user) {
        signalRService.offNotification(handleNotification);
      }
    };
  }, [user]);

  const login = async (email: string, password: string) => {
    setIsLoading(true);
    try {
      const user = await loginMutation.mutateAsync({ email, password });
      setUser(user);
      const token = localStorage.getItem('accessToken');
      if (token) {
        await signalRService.startConnection(token);
      }
    } finally {
      setIsLoading(false);
    }
  };

  const logout = async () => {
    await logoutMutation.mutateAsync();
    await signalRService.stopConnection();
    setUser(null);
    setNotifications([]);
  };

  const authenticatedQuery = useAuthenticatedQuery;

  const value = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    logout,
    authenticatedFetch,
    authenticatedQuery,
    notifications,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const useRequireAuth = (redirectUrl = '/sign-in') => {
  const { isAuthenticated, isLoading } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      navigate(redirectUrl);
    }
  }, [isAuthenticated, isLoading, navigate, redirectUrl]);

  return { isLoading };
};