import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth, useRequireAuth } from 'src/contexts/AuthContext';

interface ProtectedRouteProps {
    children: React.ReactNode;
    fallback: JSX.Element;
    requiredRoles?: ('admin' | 'user' | 'Branch Manager')[];
}

function ProtectedRoute({ children, requiredRoles, fallback }: ProtectedRouteProps) {
    const { isLoading } = useRequireAuth();
    const { user } = useAuth();


    if (isLoading) {
        return fallback;
    }

    if (!user) {
        return <Navigate to="/sign-in" replace />;
    }

    if (requiredRoles && !requiredRoles.some(role => user.roles.includes(role))) {
        return <Navigate to="/" replace />;
    }

    return <>{children}</>;
}

export default ProtectedRoute;