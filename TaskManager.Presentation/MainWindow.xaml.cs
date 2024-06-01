using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TaskManager.Business;
using TaskManager.Data;
using System.Windows.Threading;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media;
using System.Linq;

namespace TaskManager.Presentation
{
    public partial class MainWindow : Window
    {
        private readonly TaskService taskService;
        private readonly UserService userService;
        private ToDoTask selectedTask;
        private DispatcherTimer _timer;
        private NotificationManager _notificationManager;
        private List<ToDoTask> allTasks;
        public string UserDisplayName
        {
            get => userNameTextBlock.Text;
            set => userNameTextBlock.Text = value;
        }

        public MainWindow(TaskService taskService, UserService userService)
        {
            InitializeComponent();
            this.taskService = taskService;
            this.userService = userService;
            _notificationManager = new NotificationManager(NotificationHost); 
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30) // 1 dakika aralıklarla kontrol et
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            LoadTasksAsync();

        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            var tasks = await taskService.GetTasksAsync();
            var now = DateTime.Now;

            foreach (var task in tasks)
            {
                var reminderTime = task.DUE_DATE.AddMinutes(-task.REMINDER_TIME);
                if (task.DUE_DATE > now && reminderTime <= now && !task.IS_COMPLETED)
                {
                    _notificationManager.ShowNotification(
                        title: "Görev Hatırlatma",
                        message: $"Görev: {task.TITLE} zamanı yaklaşıyor.",
                        displayDurationInSeconds: 10, // 10 saniye göster
                        onClick: () => ShowTaskDetails(task)
                    );
                }
            }
        }

        private void ShowTaskDetails(ToDoTask task)
        {
            selectedTask = task;
            titleTextBox.Text = task.TITLE;
            descriptionTextBox.Text = task.DESCRIPTION;
            dueDatePicker.SelectedDate = task.DUE_DATE;
            reminderTimeTextBox.Text = task.REMINDER_TIME.ToString();
            isCompletedCheckBox.IsChecked = task.IS_COMPLETED;
        }

        private async Task LoadTasksAsync()
        {
            try
            {
                allTasks = await taskService.GetTasksAsync();
                tasksDataGrid.ItemsSource = allTasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veriler yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

         private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (filterTagTextBox == null)
            {
               return;
            }
            if ((filterComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString() == "TagFilter")
            {
                filterTagTextBox.Visibility = Visibility.Visible;
                TagBlock.Visibility = Visibility.Visible;
            }
            else
            {
                filterTagTextBox.Visibility = Visibility.Collapsed;
                TagBlock.Visibility = Visibility.Collapsed;
                FilterTasks();

            }

           
        }

        private void FilterTasks()
        {
            if (filterComboBox.SelectedItem == null)
            {
                tasksDataGrid.ItemsSource = allTasks;
                return;
            }

            var selectedTag = (filterComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
            List<ToDoTask> filteredTasks;

            switch (selectedTag)
            {
                case "Completed":
                    filteredTasks = allTasks.Where(task => task.IS_COMPLETED).ToList();
                    break;
                case "NotCompleted":
                    filteredTasks = allTasks.Where(task => !task.IS_COMPLETED).ToList();
                    break;
                case "TagFilter":
                    var filterTag = filterTagTextBox.Text.ToLower();
                    filteredTasks = allTasks.Where(task => task.TAGS?.ToLower().Contains(filterTag) == true).ToList();
                    break;
                default:
                    filteredTasks = allTasks;
                    break;
            }

            tasksDataGrid.ItemsSource = filteredTasks;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ToDoTask newTask = new ToDoTask
                {
                    TITLE = titleTextBox.Text,
                    DESCRIPTION = descriptionTextBox.Text,
                    TAGS = tagsTextBox.Text,
                    DUE_DATE = dueDatePicker.SelectedDate ?? DateTime.Now,
                    REMINDER_TIME = int.TryParse(reminderTimeTextBox.Text, out int reminder) ? reminder : 0,
                    IS_COMPLETED = isCompletedCheckBox.IsChecked ?? false,
                    PRIORITY= int.Parse((priorityComboBox.SelectedItem as ComboBoxItem).Tag.ToString())
                };

                await taskService.AddTaskAsync(newTask);
                await LoadTasksAsync();
                ClearForm();
                MessageBox.Show("Görev başarıyla eklendi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görev eklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTask != null)
            {
                var result = MessageBox.Show("Bu görevi güncellemek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        selectedTask.TITLE = titleTextBox.Text;
                        selectedTask.DESCRIPTION = descriptionTextBox.Text;
                        selectedTask.TAGS=tagsTextBox.Text;
                        selectedTask.DUE_DATE = dueDatePicker.SelectedDate ?? DateTime.Now;
                        selectedTask.REMINDER_TIME = int.TryParse(reminderTimeTextBox.Text, out int reminder) ? reminder : 0;
                        selectedTask.IS_COMPLETED = isCompletedCheckBox.IsChecked ?? false;
                        selectedTask.PRIORITY = int.Parse((priorityComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

                        await taskService.UpdateTaskAsync(selectedTask);
                        await LoadTasksAsync();
                        ClearForm();
                        MessageBox.Show("Görev başarıyla güncellendi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Görev güncellenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private async void CompleteTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (tasksDataGrid.SelectedItem is ToDoTask task)
            {
                task.IS_COMPLETED = true;
                try
                {
                    await taskService.UpdateTaskAsync(task);
                    await LoadTasksAsync();
                    _notificationManager.ShowNotification(
                        title: "Görev Tamamlandı",
                        message: $"Görev: {task.TITLE} başarıyla tamamlandı.",
                        displayDurationInSeconds: 10 // 10 saniye göster
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Görev güncellenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTask != null)
            {
                var result = MessageBox.Show("Bu görevi silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await taskService.DeleteTaskAsync(selectedTask.TASK_ID);
                        await LoadTasksAsync();
                        ClearForm();
                        MessageBox.Show("İşlem Başarılı", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"İşlem başarısız: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void TasksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tasksDataGrid.SelectedItem is ToDoTask task)
            {
                selectedTask = task;
                titleTextBox.Text = task.TITLE;
                descriptionTextBox.Text = task.DESCRIPTION;
                dueDatePicker.SelectedDate = task.DUE_DATE;
                reminderTimeTextBox.Text = task.REMINDER_TIME.ToString();
                isCompletedCheckBox.IsChecked = task.IS_COMPLETED;
                foreach (ComboBoxItem item in priorityComboBox.Items)
                {
                    if (item.Tag.ToString() == task.PRIORITY.ToString())
                    {
                        priorityComboBox.SelectedItem = item;
                        break;
                    }
                }
                tagsTextBox.Text = task.TAGS;


                LoadCommentsAsync(selectedTask.TASK_ID);
            }

           
        }

        private void TasksDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tasksDataGrid.SelectedItem is ToDoTask task)
            {
                _notificationManager.ShowNotification(
                    title: "Görev Detayı",
                    message: $"Görev: {task.TITLE}",
                    displayDurationInSeconds: 10, // 10 saniye göster
                    onClick: () => ShowTaskDetails(task)
                );
            }
        }

        private void ClearForm()
        {
            titleTextBox.Clear();
            descriptionTextBox.Clear();
            dueDatePicker.SelectedDate = DateTime.Now;
            reminderTimeTextBox.Clear();
            isCompletedCheckBox.IsChecked = false;
            selectedTask = null;
        }
        private void DataGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            tasksDataGrid.Cursor = Cursors.Hand;
        }

        private void DataGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            tasksDataGrid.Cursor = Cursors.Arrow;
        }
        private async void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var tasks = await taskService.GetTasksAsync();
            byte[] excelData = ExcelExporter.ExportTasksToExcel(tasks);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, excelData);
                MessageBox.Show("Görevler başarıyla Excel dosyasına aktarıldı.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void ExportToPdfButton_Click(object sender, RoutedEventArgs e)
        {
            var tasks = await taskService.GetTasksAsync();
            byte[] pdfData = PdfExporter.ExportTasksToPdf(tasks);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                DefaultExt = "pdf",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, pdfData);
                MessageBox.Show("Görevler başarıyla PDF dosyasına aktarıldı.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = (ToDoTask)tasksDataGrid.SelectedItem;
            if (selectedTask != null)
            {
                string commentText = newCommentTextBox.Text;
                if (!string.IsNullOrWhiteSpace(commentText))
                {
                    await taskService.AddCommentAsync(selectedTask.TASK_ID, GlobalVariables.LOGIN_ID, commentText);
                    newCommentTextBox.Clear();
                    LoadCommentsAsync(selectedTask.TASK_ID);
                }
                else
                {
                    MessageBox.Show("Yorum metni boş olamaz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ShareTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTask != null && int.TryParse(shareWithUserIdTextBox.Text, out int userId))
            {
                await taskService.ShareTaskAsync(selectedTask.TASK_ID, userId);
                shareWithUserIdTextBox.Clear();
            }
        }

        private async void LoadCommentsAsync(int taskId)
        {
            var comments = await taskService.GetCommentsAsync(taskId);
            commentsListBox.Items.Clear();
            foreach (var comment in comments)
            {
                var userFullName = $"{comment.USER_NAME} {comment.USER_SURNAME}";
                var commentText = $"{userFullName} : {comment.COMMENT_TEXT}";

                var listBoxItem = new ListBoxItem
                {
                    Content = commentText,
                    Background = GetUserColor(comment.USER_ID)
                };

                commentsListBox.Items.Add(listBoxItem);
            }
        }
        private readonly List<SolidColorBrush> _colorPalette = new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromRgb(255, 235, 205)), // Açık Turuncu
            new SolidColorBrush(Color.FromRgb(255, 228, 225)), // Açık Pembe
            new SolidColorBrush(Color.FromRgb(240, 255, 240)), // Açık Yeşil
            new SolidColorBrush(Color.FromRgb(240, 248, 255)), // Açık Mavi
            new SolidColorBrush(Color.FromRgb(255, 240, 245)), // Açık Lavanta
            new SolidColorBrush(Color.FromRgb(245, 245, 220)), // Açık Bej
            new SolidColorBrush(Color.FromRgb(255, 250, 205)), // Açık Altın
            new SolidColorBrush(Color.FromRgb(255, 245, 238)), // Açık Mercan
            new SolidColorBrush(Color.FromRgb(255, 255, 224)), // Açık Limon
            new SolidColorBrush(Color.FromRgb(255, 255, 240)), // Açık Fildişi
        };

        private SolidColorBrush GetUserColor(int userId)
        {
            int colorIndex = userId % _colorPalette.Count;
            return _colorPalette[colorIndex];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            this.KeyDown += new KeyEventHandler(OnKeyDown);
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
           
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                this.ResizeMode = ResizeMode.CanResize;
            }
        }
    }
}
