using System;
using System.Windows;
using System.Windows.Input;
using OSSimulation.ViewModels;

namespace OSSimulation
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // Create and set the ViewModel
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        // ========== WINDOW CONTROLS ==========

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // ========== BUTTON HANDLERS ==========

        private void CreateProcess_Click(object sender, RoutedEventArgs e)
        {
            // For now, create a simple test process
            _viewModel.CreateProcess(
                name: "Test Process",
                burstTime: 500,
                priority: 5,
                memoryMB: 100
            );
        }

        private void AutoGenerate_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AutoGenerateProcesses();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.StartSimulation();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PauseSimulation();
        }
    }
}