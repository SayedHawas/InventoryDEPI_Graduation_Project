import { lazy, Suspense } from 'react';
import { Outlet, Navigate, useRoutes } from 'react-router-dom';

import Box from '@mui/material/Box';
import LinearProgress, { linearProgressClasses } from '@mui/material/LinearProgress';

import { varAlpha } from 'src/theme/styles';
import { AuthLayout } from 'src/layouts/auth';
import { DashboardLayout } from 'src/layouts/dashboard';
import ProtectedRoute from './components/protected-route';

// ----------------------------------------------------------------------

export const HomePage = lazy(() => import('src/pages/home'));
export const BlogPage = lazy(() => import('src/pages/blog'));
export const UserPage = lazy(() => import('src/pages/user'));
export const AttributePage = lazy(() => import('src/pages/attribute'));
export const BrandPage = lazy(() => import('src/pages/brand'));
export const BranchPage = lazy(() => import('src/pages/branch'));
export const StorageLocationPage = lazy(() => import('src/pages/storage-location'));
export const CategoryPage = lazy(() => import('src/pages/category'));
export const SignInPage = lazy(() => import('src/pages/sign-in'));
export const ProductsPage = lazy(() => import('src/pages/products'));
export const SuppliersPage = lazy(() => import('src/pages/supplier'));
export const ProcurementPage = lazy(() => import('src/pages/transaction/procurement'));
export const SalesPage = lazy(() => import('src/pages/transaction/sales'));
export const AdjustmentPage = lazy(() => import('src/pages/transaction/adjustment'));
export const Page404 = lazy(() => import('src/pages/page-not-found'));

// ----------------------------------------------------------------------

const renderFallback = (
  <Box display="flex" alignItems="center" justifyContent="center" flex="1 1 auto">
    <LinearProgress
      sx={{
        width: 1,
        maxWidth: 320,
        bgcolor: (theme) => varAlpha(theme.vars.palette.text.primaryChannel, 0.16),
        [`& .${linearProgressClasses.bar}`]: { bgcolor: 'text.primary' },
      }}
    />
  </Box>
);

export function Router() {
  return useRoutes([
    {
      element: (
          <ProtectedRoute fallback={renderFallback}>
            <DashboardLayout>
              <Suspense fallback={renderFallback}>
                <Outlet />
              </Suspense>
            </DashboardLayout>
          </ProtectedRoute>
      ),
      children: [
        { element: <HomePage />, index: true },
        { path: 'user', element: <UserPage /> },
        { path: 'brand', element: <BrandPage /> },
        { path: 'attribute', element: <AttributePage /> },
        { path: 'branch', element: <BranchPage /> },
        { path: 'storage-location', element: <StorageLocationPage /> },
        { path: 'category', element: <CategoryPage /> },
        { path: 'products', element: <ProductsPage /> },
        { path: 'supplier', element: <SuppliersPage /> },
        { path: 'transactions/procurement', element: <ProcurementPage /> },
        { path: 'transactions/sales', element: <SalesPage /> },
        { path: 'transactions/adjustment', element: <AdjustmentPage /> },
        { path: 'supplier', element: <SuppliersPage /> },
        { path: 'blog', element: <BlogPage /> },
      ],
    },
    {
      element: (
          <ProtectedRoute requiredRoles={['Branch Manager', 'admin']} fallback={renderFallback}>
            <DashboardLayout>
              <Suspense fallback={renderFallback}>
                <Outlet />
              </Suspense>
            </DashboardLayout>
          </ProtectedRoute>
      ),
      children: [
        { element: <HomePage />, index: true },
        { path: 'user', element: <UserPage /> },
      ],
    },
    {
      element: (
          <ProtectedRoute requiredRoles={['admin']} fallback={renderFallback}>
            <DashboardLayout>
              <Suspense fallback={renderFallback}>
                <Outlet />
              </Suspense>
            </DashboardLayout>
          </ProtectedRoute>
      ),
      children: [
        { element: <HomePage />, index: true },
        { path: 'branch', element: <BranchPage /> },
      ],
    },
    {
      path: 'sign-in',
      element: (
        <AuthLayout>
          <SignInPage />
        </AuthLayout>
      ),
    },
    {
      path: '404',
      element: <Page404 />,
    },
    {
      path: '*',
      element: <Navigate to="/404" replace />,
    },
  ]);
}
