export type AlertType = 'success' | 'danger';
export type AlertState = {
    show: boolean;
    type: AlertType;
    message: string;
};

export function getNotificationIcon(type: AlertType): string {
    return type === 'success' ? 'bi bi-check-circle me-2' : 'bi bi-exclamation-octagon me-2';
};