using HexadEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace HexadEditor.Components
{
    [DataContract]
    class Transform : Component
    {
        // Position
        private Vector3D _position;
        [DataMember]
        public Vector3D Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        // Rotation
        private Vector3D _rotation;
        [DataMember]
        public Vector3D Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }

        // Scale
        private Vector3D _scale;
        [DataMember]
        public Vector3D Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }
        }

        // Returns the multi-select variant of a component
        public override IMSComponent GetMultiselectionComponent(MSEntity msEntity) => new MSTransform(msEntity);

        public Transform(GameEntity owner) : base(owner) 
        { 
            
        }
    }

    sealed class MSTransform : MSComponent<Transform>
    {
        #region transform properties
        // X position
        private float? _posX;
        public float? PosX
        {
            get => _posX;
            set
            {
                if (_posX.IsTheSameAs(value)) // Approximates floating point values using utilities
                {
                    _posX = value;
                    OnPropertyChanged(nameof(PosX));
                }
            }
        }

        // Y position
        private float? _posY;
        public float? PosY
        {
            get => _posY;
            set
            {
                if (_posY.IsTheSameAs(value)) // Approximates floating point values using utilities
                {
                    _posY = value;
                    OnPropertyChanged(nameof(PosY));
                }
            }
        }

        // Z position
        private float? _posZ;
        public float? PosZ
        {
            get => _posZ;
            set
            {
                if (_posZ.IsTheSameAs(value)) // Approximates floating point values using utilities
                {
                    _posZ = value;
                    OnPropertyChanged(nameof(PosZ));
                }
            }
        }

        // X rotation
        private float? _rotX;
        public float? RotX
        {
            get => _rotX;
            set
            {
                if (_rotX.IsTheSameAs(value)) // Approximates floating point values using utilities
                {
                    _rotX = value;
                    OnPropertyChanged(nameof(RotX));
                }
            }
        }

        // Y rotation
        private float? _rotY;
        public float? RotY
        {
            get => _rotY;
            set
            {
                if (_rotY.IsTheSameAs(value)) // Approximates floating point values using utilities
                {
                    _rotY = value;
                    OnPropertyChanged(nameof(RotY));
                }
            }
        }

        // Z rotation
        private float? _rotZ;
        public float? RotZ
        {
            get => _rotZ;
            set
            {
                if (_rotZ.IsTheSameAs(value)) // Approximates floating point values using utilities
                {
                    _rotZ = value;
                    OnPropertyChanged(nameof(RotZ));
                }
            }
        }

        // X scale
        private float? _scaleX;
        public float? ScaleX
        {
            get => _scaleX;
            set
            {
                if (_scaleX.IsTheSameAs(value)) // Approximates floating point values using utilities
                {
                    _scaleX = value;
                    OnPropertyChanged(nameof(ScaleX));
                }
            }
        }

        // Y scale
        private float? _scaleY;
        public float? ScaleY
        {
            get => _scaleY;
            set
            {
                if (_scaleY.IsTheSameAs(value)) // Approximates floating point values using utilities
                {
                    _scaleY = value;
                    OnPropertyChanged(nameof(ScaleY));
                }
            }
        }

        // X scale
        private float? _scaleZ;
        public float? ScaleZ
        {
            get => _scaleZ;
            set
            {
                if (_scaleZ.IsTheSameAs(value)) // Approximates floating point values using utilities
                {
                    _scaleZ = value;
                    OnPropertyChanged(nameof(ScaleZ));
                }
            }
        }
        #endregion

        protected override bool UpdateComponents(string propertyName)
        {
            // If any of the component's properties were changed, create a new Vector3 containing the new relevant information
            switch (propertyName)
            {
                case nameof(PosX):
                case nameof(PosY):
                case nameof(PosZ):
                    SelectedComponents.ForEach(c => c.Position = new Vector3D(_posX ?? c.Position.X, _posY ?? c.Position.Y, _posZ ?? c.Position.Z));
                    return true;

                case nameof(RotX):
                case nameof(RotY):
                case nameof(RotZ):
                    SelectedComponents.ForEach(c => c.Rotation = new Vector3D(_rotX ?? c.Rotation.X, _rotY ?? c.Rotation.Y, _rotZ ?? c.Rotation.Z));
                    return true;

                case nameof(ScaleX):
                case nameof(ScaleY):
                case nameof(ScaleZ):
                    SelectedComponents.ForEach(c => c.Scale = new Vector3D(_scaleX ?? c.Scale.X, _scaleY ?? c.Scale.Y, _scaleZ ?? c.Scale.Z));
                    return true;
            }
            return false;
        }

        protected override bool UpdateMSComponents()
        {
            PosX = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => (float)x.Position.X));
            PosY = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(y => (float)y.Position.Y));
            PosZ = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(z => (float)z.Position.Z));

            RotX = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => (float)x.Rotation.X));
            RotY = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(y => (float)y.Rotation.Y));
            RotZ = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(z => (float)z.Rotation.Z));

            ScaleX = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => (float)x.Scale.X));
            ScaleY = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(y => (float)y.Scale.Y));
            ScaleZ = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(z => (float)z.Scale.Z));

            return true;
        }

        public MSTransform(MSEntity msEntity) : base(msEntity)
        {
            Refresh();
        }
    }
}
