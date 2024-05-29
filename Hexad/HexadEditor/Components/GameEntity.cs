using HexadEditor.DllWrappers;
using HexadEditor.GameProject;
using HexadEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HexadEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    class GameEntity : ViewModelBase
    {
        private int _entityId = ID.INVALID_ID;
        public int EntityId
        {
            get => _entityId;
            set 
            {
                if (_entityId != value) 
                {
                    _entityId = value;
                    OnPropertyChanged(nameof(EntityId));
                }
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set 
            { 
                if (_isActive != value)
                {
                    _isActive = value;
                    if (_isActive)
                    {
                        EntityId = EngineAPI.CreateGameEntity(this);
                        Debug.Assert(ID.IsValid(_entityId));
                    }
                    else
                    {
                        EngineAPI.RemoveGameEntity(this);
                    }
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        private bool _isEnabled = true;
        [DataMember]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set 
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string _name;
        [DataMember]
        public string Name 
        { 
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            } 
        }

        [DataMember]
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        public Component GetComponent(Type type) => (Component)Components.FirstOrDefault(c => c.GetType() == type);
        public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T; // shortens what needs to be typed out to call the above method

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }
        }

        // Constructor
        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;

            // All objects have a transform component
            _components.Add(new Transform(this));

            // Makes sure the observable collection is linked with the newly added components from the constructor
            OnDeserialized(new StreamingContext());
        }
    }

    // Multiselection constructor
    // Called whenever we make a new selection
    abstract class MSEntity : ViewModelBase
    {
        // Enables updates to selected entities
        private bool _enableUpdates = true;

        private bool? _isEnabled = true; // is true if true for all, is false if false for all, if mixed true/false then is null
        [DataMember]
        public bool? IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public List<GameEntity> SelectedEntities { get; }

        private readonly ObservableCollection<IMSComponent> _componenets = new ObservableCollection<IMSComponent>();
        public ReadOnlyObservableCollection<IMSComponent> Components { get; }

        // Returns a float, bool, or string so long as all selected values are the same
        // Returns NULL if not
        private static float? GetMixedValue(List<GameEntity> entities, Func<GameEntity, float> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
                if (value.IsTheSameAs(getProperty(entity)))
                {
                    return null;
                }
            }
            return value;
        }
        private static bool? GetMixedValue(List<GameEntity> entities, Func<GameEntity, bool> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
                if (value != getProperty(entity))
                {
                    return null;
                }
            }
            return value;
        }
        private static string GetMixedValue(List<GameEntity> entities, Func<GameEntity, string> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
                if (value != getProperty(entity))
                {
                    return null;
                }
            }
            return value;
        }

        protected virtual bool UpdateGameEntities(string propertyName)
        {
            switch (propertyName) // updates the value of corresponding properties
            {
                case nameof(IsEnabled): SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value); return true;
                case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true;
            }
            return false; // nothing to handle in the base class
        }

        protected virtual bool UpdateMSGameEntity()
        {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

            return true;
        }

        // Reads selected entities and their properties and adds to the MS properties
        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSGameEntity();
            _enableUpdates = true;
        }

        public MSEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities?.Any() == true);
            Components = new ReadOnlyObservableCollection<IMSComponent>(_componenets);
            SelectedEntities = entities;
            PropertyChanged += (s, e) => { if(_enableUpdates) UpdateGameEntities(e.PropertyName); };
        }
    }

    class MSGameEntity : MSEntity
    {
        public MSGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh();
        }
    }
}
