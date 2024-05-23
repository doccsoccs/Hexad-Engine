using HexadEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HexadEditor.GameProject
{
    [DataContract(Name = "Game")]
    class Project : ViewModelBase
    {
        public static string Extension { get; } = ".hexad";

        [DataMember]
        public string Name { get; private set; } = "New Project";

        [DataMember]
        public string Path { get; private set; }

        public string FullPath => $@"{Path}{Name}\{Name}{Extension}";

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        private Scene _activeScene;
        public Scene ActiveScene
        {
            get { return _activeScene; }
            set 
            { 
                if (_activeScene != value)
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(_activeScene));
                }
            }
        }

        public static Project Current => Application.Current.MainWindow.DataContext as Project;

        public static UndoRedo UndoRedo { get; } = new UndoRedo();

        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }

        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveSceneCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Adds a scene with a given name to the project
        /// </summary>
        /// <param name="sceneName"></param>
        public void AddScene(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }

        /// <summary>
        /// Removes a scene from the project
        /// </summary>
        /// <param name="scene"></param>
        public void RemoveScene(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }

        public void Unload()
        {
            UndoRedo.Reset(); // don't retain UNDO/REDO info between saves
        }

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public static void Save(Project project)
        {
            Serializer.ToFile(project, project.FullPath); // Write to .hexad file
            Logger.Log(MessageType.Info, $"Saved project to {project.FullPath}");
        }

        [OnDeserialized] // Called when project instance is created
        private void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

            // Undo/Redo for adding a scene
            AddSceneCommand = new RelayCommand<object>(x =>
            {
                AddScene($"New Scene {_scenes.Count}");
                var newScene = _scenes.Last();
                var sceneIndex = _scenes.Count - 1;

                // Creates an undoredo with actions that:
                // 1) Undo - removes the new scene from the scenes list by calling the remove scene function
                // 2) Redo - inserts the new scene back into the scenes list via the saved index
                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => _scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"));
            });

            // Undo/Redo for removing a scene
            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                var sceneIndex = _scenes.IndexOf(x);
                RemoveScene(x);

                // Creates an undoredo with actions that:
                // 1) Undo - adds back the removed scene
                // 2) Redo - removes the scene again if undone
                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveScene(x),
                    $"Removed {x.Name}"));
            }, x => !x.IsActive); // <-- DO NOT REMOVE ACTIVE SCENES

            UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo());
            RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo());
            SaveCommand = new RelayCommand<object>(x => Save(this));
        }

        // Constructor
        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
        }
    }
}
