﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HexadEditor.Dictionary
{
    public partial class ControlTemplates : ResourceDictionary
    {
        private void OnTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            var exp = textBox.GetBindingExpression(TextBox.TextProperty);
            if (exp == null) return;

            // Accept new value
            if (e.Key == Key.Enter)
            {
                if(textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
                {
                    command.Execute(textBox.Text);
                }
                else
                {
                    exp.UpdateSource();
                }
                Keyboard.ClearFocus();
                e.Handled = true;
            }

            // Keep old value and lose focus from textbox
            else if (e.Key == Key.Escape)
            {
                exp.UpdateTarget();
                Keyboard.ClearFocus();
            }
        }

        private void OnTextBoxRename_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            var exp = textBox.GetBindingExpression(TextBox.TextProperty);
            if (exp == null) return;

            // Accept new value
            if (e.Key == Key.Enter)
            {
                if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
                {
                    command.Execute(textBox.Text);
                }
                else
                {
                    exp.UpdateSource();
                }
                textBox.Visibility = Visibility.Visible;
                e.Handled = true;
            }

            // Keep old value and lose focus from textbox
            else if (e.Key == Key.Escape)
            {
                exp.UpdateTarget();
                textBox.Visibility = Visibility.Collapsed;
            }
        }

        private void OnTextBoxRename_LostFocus(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            var exp = textBox.GetBindingExpression(TextBox.TextProperty);
            if (exp != null)
            {
                exp.UpdateTarget();
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                textBox.Visibility = Visibility.Collapsed;
            }
        }

        // Closes the editor window
        private void OnClose_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.Close();
        }

        // Maximize / Restore the editor window
        private void OnMaximizeRestore_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowState = (window.WindowState == WindowState.Normal) ?
                WindowState.Maximized : WindowState.Normal;
        }

        // Minimize the editor window
        private void OnMinimize_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowState = WindowState.Minimized;
        }
    }
}
