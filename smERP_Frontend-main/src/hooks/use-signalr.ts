import { useState, useEffect, useCallback } from 'react';
import { signalRService } from '../services/signalR';
import { Notification } from '../services/types';
import { useAuth } from '../contexts/AuthContext';

export const useSignalR = () => {
    const [notifications, setNotifications] = useState<Notification[]>([]);
    const { user, isAuthenticated } = useAuth();

    const handleNotification = useCallback((newNotifications: Notification | Notification[]) => {
        console.log('Incoming notification(s):', newNotifications); // Log incoming notifications
        setNotifications(prev => [
            ...(Array.isArray(newNotifications) ? newNotifications : [newNotifications]),
            ...prev
        ]);
    }, []);

    useEffect(() => {
        if (isAuthenticated && user) {
            const accessToken = localStorage.getItem('accessToken') || '';
            signalRService.startConnection(accessToken).then(() => {
                console.log('SignalR connection started');
                signalRService.onNotification(handleNotification);
            }).catch((error) => {
                console.error('Failed to start SignalR connection:', error);
            });

            return () => {
                console.log('Stopping SignalR connection');
                signalRService.offNotification(handleNotification);
                signalRService.stopConnection();
            };
        }
    }, [isAuthenticated, user, handleNotification]);

    return { notifications };
};