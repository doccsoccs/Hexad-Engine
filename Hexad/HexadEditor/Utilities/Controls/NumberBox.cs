﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HexadEditor.Utilities.Controls
{
    [TemplatePart(Name = "PART_textBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_textBox", Type = typeof(TextBox))]
    class NumberBox : Control
    {
        private double _originalValue;
        private double _mouseXStart;
        private double _multiplier; // raise and lower with holding CTRL or SHIFT
        private bool _captured = false;
        private bool _valueChanged = false;

        public double Multiplier
        {
            get => (double)GetValue(MultiplierProperty);
            set => SetValue(MultiplierProperty, value);
        }
        public static readonly DependencyProperty MultiplierProperty =
            DependencyProperty.Register(nameof(Multiplier), typeof(double), typeof(NumberBox),
                new PropertyMetadata(1.0));

        // Property (backing field)
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        // Dependency Property --> register and connect to regular property
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumberBox), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get a template part and attach event handlers to it
            if(GetTemplateChild("PART_textBlock") is TextBlock textBlock)
            {
                textBlock.MouseLeftButtonDown += OnTextBlock_Mouse_LBD;
                textBlock.MouseLeftButtonUp += OnTextBlock_Mouse_LBU;
                textBlock.MouseMove += OnTextBlock_Mouse_Move;
            }
        }

        // On Initial Left Click on a NumberBox Element
        private void OnTextBlock_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            double.TryParse(Value, out _originalValue);

            Mouse.Capture(sender as UIElement);
            _captured = true;
            _valueChanged = false;
            e.Handled = true;

            // Get the current mouse's X position relative to THIS control
            _multiplier = 0.01;
            _mouseXStart = e.GetPosition(this).X;
        }

        // On Release Left Click on a NumberBox Element
        // HOW IT KNOWS WHEN YOU WANT TO TYPE IN A NUMBER BY HAND
        private void OnTextBlock_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            if (_captured)
            {
                Mouse.Capture(null);
                _captured = false;
                e.Handled = true;
                if (!_valueChanged && GetTemplateChild("PART_textBox") is TextBox textBox)
                {
                    textBox.Visibility = Visibility.Visible;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        // When moving the mouse left or right while holding down
        private void OnTextBlock_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (_captured)
            {
                var mouseX = e.GetPosition(this).X;
                var distance = mouseX - _mouseXStart;
                if (Math.Abs(distance) > SystemParameters.MinimumHorizontalDragDistance)
                {
                    // Multiplier modifiers
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) _multiplier = 0.001;
                    else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) _multiplier = 0.1;
                    else { _multiplier = 0.01; };

                    var newValue = _originalValue + (distance * _multiplier * Multiplier);
                    Value = newValue.ToString("0.##");
                    _valueChanged = true;
                }
            }
        }

        // CTOR
        static NumberBox()
        {
            // Override default style key property
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), 
                new FrameworkPropertyMetadata(typeof(NumberBox)));
        }
    }
}
