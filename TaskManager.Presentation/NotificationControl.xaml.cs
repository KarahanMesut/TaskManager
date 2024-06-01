using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TaskManager.Presentation
{
    public partial class NotificationControl : UserControl
    {
        public NotificationControl(string title, string message, int durationInSeconds)
        {
            InitializeComponent();
            TitleText.Text = title;
            MessageText.Text = message;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(durationInSeconds)
            };
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                var parent = this.Parent as Panel;
                if (parent != null)
                {
                    parent.Children.Remove(this);
                }
            };
            timer.Start();
        }
    }
}
