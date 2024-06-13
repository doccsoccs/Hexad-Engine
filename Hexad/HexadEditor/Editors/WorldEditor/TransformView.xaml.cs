using HexadEditor.Components;
using HexadEditor.GameProject;
using HexadEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HexadEditor.Editors
{
    /// <summary>
    /// Interaction logic for TransformView.xaml
    /// </summary>
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChanged = false;

        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private void OnTransformViewLoaded(object sender, RoutedEventArgs eventArgs)
        {
            Loaded -= OnTransformViewLoaded;
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
        }

        // --------------------------------------------------------------------
        // ------ UNDO REDO CONTROLS FOR TRANSFORM COMPONENT VECTORBOXES ------
        // --------------------------------------------------------------------

        // Returns an action that returns a vectorbox item to its previous state
        private Action GetAction(Func<Transform, (Transform transform, Vector3D)> selector, Action<(Transform transform, Vector3D)> forEachAction) 
        {
            // do nothing if the data context is not a transform component
            if (!(DataContext is MSTransform vm))
            {
                _undoAction = null;
                _propertyChanged = false;
                return null;
            }

            var selection = vm.SelectedComponents.Select(x => selector(x)).ToList(); // get a list of selected transforms
            return new Action(() =>
            {
                selection.ForEach(x => forEachAction(x)); // sets positions to their old value
                (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh(); // refreshes the component
            });
        }

        // Get UndoRedo Actions for transform elements Position, Rotation, or Scale
        private Action GetPositionAction() => GetAction((x) => (x, x.Position), (x) => x.transform.Position = x.Item2);
        private Action GetRotationAction() => GetAction((x) => (x, x.Rotation), (x) => x.transform.Rotation = x.Item2);
        private Action GetScaleAction() => GetAction((x) => (x, x.Scale), (x) => x.transform.Scale = x.Item2);

        // Records an action to be Undone and Redone
        private void RecordAction(Action redoAction, string name)
        {
            if (_propertyChanged) // if the property was changed
            {
                Debug.Assert(_undoAction != null);
                _propertyChanged = false;
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }

        /// <summary>
        /// POSITION
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // LEFT MOUSE BUTTON PRESSED
        private void OnPosition_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetPositionAction();
        }

        // LEFT MOUSE BUTTON RELEASED
        private void OnPosition_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetPositionAction(), "Position Changed");
        }

        // FOCUS LOST
        private void OnPosition_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnPosition_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }

        /// <summary>
        /// ROTATION
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRotation_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetRotationAction();
        }
        private void OnRotation_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetRotationAction(), "Rotation Changed");
        }
        private void OnRotation_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnRotation_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }

        /// <summary>
        /// SCALE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScale_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetScaleAction();
        }
        private void OnScale_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetScaleAction(), "Scale Changed");
        }
        private void OnScale_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnScale_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }
    }
}
