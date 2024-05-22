﻿using HexadEditor.Components;
using HexadEditor.GameProject;
using HexadEditor.Utilities;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HexadEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var vm = btn.DataContext as Scene;
            vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity"});
        }

        private void OnGameEntities_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItems.Count > 0) // Prevents getting null selection data when an object has no components
            {
                GameEntityView.Instance.DataContext = null;
                var listBox = sender as ListBox;

                if (e.AddedItems.Count > 0)
                {
                    GameEntityView.Instance.DataContext = listBox.SelectedItems[0];
                }

                var newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList();
                var previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => //undo
                    { 
                        listBox.UnselectAll();
                        previousSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                    },
                    () => //redo
                    {
                        // 
                    },
                    "Selection changed"
                ));
            }
        }
    }
}
