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
using System.IO;

namespace HexadEditor
{
    /// <summary>
    /// Interaction logic for EnginePathDialog.xaml
    /// </summary>
    public partial class EnginePathDialog : Window
    {
        public string HexadPath { get; private set; }

        public EnginePathDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void OnOk_Button_Click(object sender, RoutedEventArgs e)
        {
            var path = pathTextBox.Text.Trim();
            messageTextBlock.Text = string.Empty;
            
            // Validity Indication Messages
            if (string.IsNullOrEmpty(path))
            {
                messageTextBlock.Text = "Invalid path.";
            }
            else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                messageTextBlock.Text = "Invalid character(s) used in path.";
            }
            else if (!Directory.Exists(Path.Combine(path, @"Engine\EngineAPI\")))
            {
                messageTextBlock.Text = "Unable to find engine in specified location.";
            }

            // Adds backslash to path if missing
            if (string.IsNullOrEmpty(messageTextBlock.Text))
            {
                if (path.EndsWith(Path.DirectorySeparatorChar.ToString())) path += @"\";
                HexadPath = path;
                DialogResult = true;
                Close();
            }
        }
    }
}
