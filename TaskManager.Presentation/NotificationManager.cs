using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;

namespace TaskManager.Presentation
{
    internal class NotificationManager
    {
        private readonly Panel _notificationHost;

        public NotificationManager(Panel notificationHost)
        {
            _notificationHost = notificationHost;
        }

        public void ShowNotification(string title, string message, int displayDurationInSeconds = 5)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var notification = new NotificationControl(title, message, displayDurationInSeconds);
                _notificationHost.Children.Add(notification);
            });
        }

        public void ShowNotification(string title, string message, int displayDurationInSeconds = 5, Action onClick = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var notification = new NotificationControl(title, message, displayDurationInSeconds);

                if (onClick != null)
                {
                    notification.MouseLeftButtonUp += (s, e) => onClick();
                    notification.Cursor = System.Windows.Input.Cursors.Hand;
                }

                _notificationHost.Children.Add(notification);
            });
        }
    }
}
