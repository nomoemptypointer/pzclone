using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Game.Core.ECS
{
    public class GameObject
    {
        private static long s_latestAssignedID = 0;

        private readonly Dictionary<Type, List<Component>> _components = [];
        private SystemRegistry _registry;
        private bool _enabled = true;
        private bool _enabledInHierarchy = true;

        public string Name { get; set; }
        public ulong ID { get; }
        public Transform Transform { get; }

        public bool Enabled
        {
            get { return _enabled; }
            set { if (value != _enabled) { SetEnabled(value); } }
        }

        public bool EnabledInHierarchy => _enabledInHierarchy;

        internal static event Action<GameObject> InternalConstructed;
        internal static event Action<GameObject> InternalDestroyRequested;
        internal static event Action<GameObject> InternalDestroyCommitted;

        public event Action<GameObject> Destroyed;

        public GameObject() : this(Guid.NewGuid().ToString())
        { }

        public GameObject(string name)
        {
            Transform t = new(this);
            t.ParentChanged += OnTransformParentChanged;
            AddComponent(t);
            Transform = t;
            Name = name;
            InternalConstructed?.Invoke(this);
            ID = GetNextID();
        }

        private static ulong GetNextID()
        {
            ulong newID = unchecked((ulong)Interlocked.Increment(ref s_latestAssignedID));
            Debug.Assert(newID != 0); // Overflow
            return newID;
        }

        public void AddComponent(Component component)
        {
            var type = component.GetType();

            if (!_components.TryGetValue(type, out var list))
            {
                list = [];
                _components[type] = list;
            }

            list.Add(component);
            component.AttachToGameObject(this, _registry);
        }

        public void AddComponent<T>(T component) where T : Component => AddComponent((Component)component);

        public void RemoveAll<T>() where T : Component
        {
            var type = typeof(T);

            if (_components.TryGetValue(type, out var list))
            {
                foreach (var c in list)
                    c.InternalRemoved(_registry);

                _components.Remove(type);
            }
        }

        public void RemoveComponent(Component component)
        {
            var type = component.GetType();

            if (_components.TryGetValue(type, out var list))
            {
                if (list.Remove(component))
                {
                    component.InternalRemoved(_registry);

                    if (list.Count == 0)
                        _components.Remove(type);
                }
            }
        }

        public void RemoveComponent<T>(T component) where T : Component => RemoveComponent((Component)component);

        public Component? GetComponent(Type type)
        {
            // Exact match
            if (_components.TryGetValue(type, out var list) && list.Count > 0)
                return list[0];

            // Assignable match (base types / interfaces)
            foreach (var kvp in _components)
            {
                if (type.IsAssignableFrom(kvp.Key) && kvp.Value.Count > 0)
                    return kvp.Value[0];
            }

            return null;
        }

        public T GetComponent<T>() where T : Component => (T)GetComponent(typeof(T));

        public IEnumerable<T> GetComponentsByInterface<T>()
        {
            foreach (var kvp in _components)
            {
                foreach (var component in kvp.Value)
                {
                    if (component is T)
                    {
                        yield return (T)(object)component;
                    }
                }
            }
        }

        public T GetComponentByInterface<T>()
        {
            return GetComponentsByInterface<T>().FirstOrDefault();
        }

        internal void SetRegistry(SystemRegistry systemRegistry)
        {
            _registry = systemRegistry;
        }

        public IEnumerable<T> GetComponents<T>() where T : Component
        {
            var requestedType = typeof(T);

            // Exact type match
            if (_components.TryGetValue(requestedType, out var list))
            {
                foreach (var comp in list)
                    yield return (T)comp;
            }

            // Assignable types (derived classes)
            foreach (var kvp in _components)
            {
                if (kvp.Key == requestedType)
                    continue;

                if (requestedType.IsAssignableFrom(kvp.Key))
                {
                    foreach (var comp in kvp.Value)
                        yield return (T)comp;
                }
            }
        }

        public T GetComponentInParent<T>() where T : Component
        {
            T component;
            GameObject parent = this;
            while ((parent = parent.Transform.Parent?.GameObject) != null)
            {
                component = parent.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        public T GetComponentInParentOrSelf<T>() where T : Component
        {
            T component;
            component = GetComponentInParent<T>();
            if (component == null)
            {
                component = GetComponent<T>();
            }

            return component;
        }

        public T GetComponentInChildren<T>() where T : Component
        {
            return (T)GetComponentInChildren(typeof(T));
        }

        public Component GetComponentInChildren(Type componentType)
        {
            foreach (var child in Transform.Children)
            {
                Component ret = child.GameObject.GetComponent(componentType) ?? child.GameObject.GetComponentInChildren(componentType);
                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        public void Destroy()
        {
            InternalDestroyRequested.Invoke(this);
        }

        internal void CommitDestroy()
        {
            foreach (var child in Transform.Children.ToArray())
            {
                child.GameObject.CommitDestroy();
            }

            foreach (var componentList in _components)
            {
                foreach (var component in componentList.Value)
                {
                    component.InternalRemoved(_registry);
                }
            }

            _components.Clear();

            Destroyed?.Invoke(this);
            InternalDestroyCommitted.Invoke(this);
        }

        private void SetEnabled(bool state)
        {
            _enabled = state;

            foreach (var child in Transform.Children)
            {
                child.GameObject.HierarchyEnabledStateChanged();
            }

            HierarchyEnabledStateChanged();
        }

        private void OnTransformParentChanged(Transform t, Transform oldParent, Transform newParent)
        {
            HierarchyEnabledStateChanged();
        }

        private void HierarchyEnabledStateChanged()
        {
            bool newState = _enabled && IsParentEnabled();
            if (_enabledInHierarchy != newState)
            {
                CoreHierarchyEnabledStateChanged(newState);
            }
        }

        private void CoreHierarchyEnabledStateChanged(bool newState)
        {
            Debug.Assert(newState != _enabledInHierarchy);
            _enabledInHierarchy = newState;
            foreach (var component in GetComponents<Component>())
            {
                component.HierarchyEnabledStateChanged();
            }
        }

        private bool IsParentEnabled() => Transform.Parent == null || Transform.Parent.GameObject.Enabled;

        public override string ToString() => $"{Name}, {_components.Values.Sum(irc => irc.Count)} components";
    }
}
