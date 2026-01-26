using Game.Core.ECS.Components;
using System.Diagnostics;

namespace Game.Core.ECS
{
    public abstract class EcsComponent
    {
        public EcsEntity Entity { get; internal set; }
        public Transform Transform { get; internal set; }

        private bool _enabled = true;
        private bool _enabledInHierarchy = false;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;
                    HierarchyEnabledStateChanged();
                }
            }
        }

        public bool EnabledInHierarchy => _enabledInHierarchy;

        internal void AttachToEntity(EcsEntity entity, SystemRegistry registry)
        {
            Entity = entity;
            Transform = Entity.Transform;
            InternalAttached(registry);
        }

        internal void HierarchyEnabledStateChanged()
        {
            bool newState = _enabled && Entity.Enabled;
            if (newState != _enabledInHierarchy)
            {
                CoreHierarchyEnabledStateChanged(newState);
            }
        }

        private void CoreHierarchyEnabledStateChanged(bool newState)
        {
            Debug.Assert(newState != _enabledInHierarchy);
            _enabledInHierarchy = newState;
            if (newState)
            {
                OnEnabled();
            }
            else
            {
                OnDisabled();
            }
        }

        internal void InternalAttached(SystemRegistry registry)
        {
            Attached(registry);
            HierarchyEnabledStateChanged();
        }

        internal void InternalRemoved(SystemRegistry registry)
        {
            if (_enabledInHierarchy)
            {
                OnDisabled();
            }

            Removed(registry);
        }

        protected abstract void OnEnabled();
        protected abstract void OnDisabled();

        protected abstract void Attached(SystemRegistry registry);
        protected abstract void Removed(SystemRegistry registry);

        public override string ToString() => GetType().Name;
    }
}