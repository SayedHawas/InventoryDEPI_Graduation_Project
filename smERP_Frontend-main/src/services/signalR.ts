import * as signalR from '@microsoft/signalr';
import { API_BASE_URL1 } from './api';

class SignalRService {
  private connection: signalR.HubConnection | null = null;

  public async startConnection(accessToken: string): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
    }

    this.connection = new signalR.HubConnectionBuilder()
    .withUrl(`${API_BASE_URL1}notifications`, {
      accessTokenFactory: () => accessToken,
      headers: {
        'Authorization': `Bearer ${accessToken}`,
        'access_token': `Bearer ${accessToken}`
      }
    })
    .withAutomaticReconnect()
    .build();

    try {
      await this.connection.start();
      console.log('SignalR Connected');
    } catch (err) {
      console.error('Error while establishing connection: ', err);
    }
  }

  public async stopConnection(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
  }

  public onNotification(callback: (notifications: any) => void): void {
    if (this.connection) {
      this.connection.on('notification', callback);
    }
  }

  public offNotification(callback: (notifications: any) => void): void {
    if (this.connection) {
      this.connection.off('notification', callback);
    }
  }
}

export const signalRService = new SignalRService();