﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HexadEditor.GameProject
{
    /// <summary>
    /// Interaction logic for CreateProjectView.xaml
    /// </summary>
    public partial class CreateProjectView : UserControl
    {
        public CreateProjectView()
        {
            InitializeComponent();

            // The most recent template in the create projects window is automatically focused
            Loaded += (s, e) =>
            {
                var item = templateListBox.ItemContainerGenerator.ContainerFromIndex(templateListBox.SelectedIndex) as ListBoxItem;
                item?.Focus();
            };
        }

        private void OnCreate_Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CreateProject;
            var projectPath = vm.CreateNewProject(templateListBox.SelectedItem as ProjectTemplate); // based on selected template

            // Close CreateProjectView Window
            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if (!string.IsNullOrEmpty(projectPath))
            {
                dialogResult = true;
                var project = OpenProject.Open(new ProjectData() { ProjectName = vm.ProjectName, ProjectPath = projectPath });
                win.DataContext = project;
            }
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
