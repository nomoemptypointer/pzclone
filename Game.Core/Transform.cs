using Game.Core.ECS;
using System.Numerics;

namespace Game.Core
{
    public class Transform : EcsComponent
    {
        //public new GameObject GameObject => base.GameObject;
        public event Action<Transform, Transform?, Transform?>? ParentChanged;

        private Vector3 _position;
        private Vector3 _rotation; // Euler angles in radians
        private Vector3 _scale = Vector3.One;

        private Transform? _parent;
        private bool _dirty = true;

        private Matrix4x4 _localMatrix;
        private Matrix4x4 _worldMatrix;

        private readonly List<Transform> _children = [];
        public IReadOnlyList<Transform> Children => _children;

        internal Transform() { }
        internal Transform(EcsEntity owner)
        {
            GameObject = owner;
        }

        public Vector3 Position
        {
            get => _position;
            set { _position = value; MarkDirty(); }
        }

        public Vector3 Rotation
        {
            get => _rotation;
            set { _rotation = value; MarkDirty(); }
        }

        public Vector3 Scale
        {
            get => _scale;
            set { _scale = value; MarkDirty(); }
        }

        public Transform? Parent
        {
            get => _parent;
            set
            {
                if (_parent == value)
                    return;

                var oldParent = _parent;

                // Remove from old parent's children
                oldParent?._children.Remove(this);

                _parent = value;

                // Add to new parent's children
                _parent?._children.Add(this);

                MarkDirty();
                ParentChanged?.Invoke(this, oldParent, _parent);
            }
        }

        public Matrix4x4 LocalMatrix
        {
            get
            {
                if (_dirty)
                    RecalculateMatrices();
                return _localMatrix;
            }
        }

        public Matrix4x4 WorldMatrix
        {
            get
            {
                if (_dirty)
                    RecalculateMatrices();
                return _worldMatrix;
            }
        }

        public Vector3 WorldPosition
            => WorldMatrix.Translation;

        private void MarkDirty()
        {
            _dirty = true;
        }

        private void RecalculateMatrices()
        {
            var rotation =
                Matrix4x4.CreateRotationX(_rotation.X) *
                Matrix4x4.CreateRotationY(_rotation.Y) *
                Matrix4x4.CreateRotationZ(_rotation.Z);

            _localMatrix =
                Matrix4x4.CreateScale(_scale) *
                rotation *
                Matrix4x4.CreateTranslation(_position);

            _worldMatrix = _parent != null
                ? _localMatrix * _parent.WorldMatrix
                : _localMatrix;

            _dirty = false;
        }

        public void Translate(Vector3 delta)
        {
            Position += delta;
        }

        public void Rotate(Vector3 delta)
        {
            Rotation += delta;
        }

        public void LookAt(Vector3 target)
        {
            Vector3 forward = Vector3.Normalize(target - WorldPosition);
            _rotation.Y = MathF.Atan2(forward.X, forward.Z);
            _rotation.X = -MathF.Asin(forward.Y);
            MarkDirty();
        }

        protected override void OnEnabled()
        {
        }

        protected override void OnDisabled()
        {
        }

        protected override void Attached(SystemRegistry registry)
        {
        }

        protected override void Removed(SystemRegistry registry)
        {
        }
    }
}
