using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TaskManagementApp
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Task> Tasks { get; set; }
        private ObservableCollection<Task> FilteredTasks { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Tasks = new ObservableCollection<Task>();
            FilteredTasks = new ObservableCollection<Task>(Tasks);
            TasksDataGrid.ItemsSource = FilteredTasks;
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            DateTime? dueDate = DueDatePicker.SelectedDate;
            string priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a task title.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Task newTask = new Task
            {
                Title = title,
                Description = description,
                DueDate = dueDate ?? DateTime.Now,
                Priority = priority ?? "Low Priority",
                IsCompleted = false
            };

            Tasks.Add(newTask);
            ApplyFilter();

            // Clear input fields
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            DueDatePicker.SelectedDate = null;
            PriorityComboBox.SelectedIndex = -1;

            MessageBox.Show("Task added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            Task selectedTask = (sender as Button)?.DataContext as Task;
            if (selectedTask != null)
            {
                TitleTextBox.Text = selectedTask.Title;
                DescriptionTextBox.Text = selectedTask.Description;
                DueDatePicker.SelectedDate = selectedTask.DueDate;
                PriorityComboBox.SelectedItem = PriorityComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == selectedTask.Priority);

                // Remove the task from the list (it will be re-added when the user clicks "Add Task")
                Tasks.Remove(selectedTask);
                ApplyFilter();
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            Task selectedTask = (sender as Button)?.DataContext as Task;
            if (selectedTask != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this task?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Tasks.Remove(selectedTask);
                    ApplyFilter();
                }
            }
        }

        private void FilterPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string selectedPriority = (FilterPriorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (selectedPriority == "All Tasks" || string.IsNullOrEmpty(selectedPriority))
            {
                FilteredTasks = new ObservableCollection<Task>(Tasks);
            }
            else
            {
                FilteredTasks = new ObservableCollection<Task>(Tasks.Where(t => t.Priority == selectedPriority));
            }

            TasksDataGrid.ItemsSource = FilteredTasks;
        }

        private void ClearCompletedTasks_Click(object sender, RoutedEventArgs e)
        {
            var completedTasks = Tasks.Where(t => t.IsCompleted).ToList();
            foreach (var task in completedTasks)
            {
                Tasks.Remove(task);
            }
            ApplyFilter();
            MessageBox.Show($"{completedTasks.Count} completed tasks have been removed.", "Tasks Cleared", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class Task
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public bool IsCompleted { get; set; }
    }
}