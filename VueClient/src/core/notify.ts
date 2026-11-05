
import { Notify, Dialog, Loading } from 'quasar';

export function msg(message: string, delaySeconds: number = 3) {
    Notify.create({
        message: message,
        position: (window.innerWidth > 768) ? 'bottom-right' : 'top',
        timeout: delaySeconds * 1000,
        multiLine: true,
        html: true,
      });
}

export function warn(message: string, delaySeconds: number = 3) {
    Notify.create({
        message: message,
        position: (window.innerWidth > 768) ? 'bottom-right' : 'top',
        timeout: delaySeconds * 1000,
        multiLine: true,
        html: true,
        color: 'warning',
      });
}