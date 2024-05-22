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
    public class Transform : Component
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

        public Transform(GameEntity owner) : base(owner) 
        { 
            
        }
    }
}
