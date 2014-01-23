using System;
using System.Collections;
using System.Collections.Generic;

namespace NHotkey.Wpf
{
    class WeakReferenceCollection<T> : IEnumerable<T>
        where T : class
    {
        private readonly List<WeakReference> _references = new List<WeakReference>();
        
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var reference in _references)
            {
                var target = reference.Target;
                if (target != null)
                    yield return (T) target;
            }
            Trim();
        }

        public void Add(T item)
        {
            _references.Add(new WeakReference(item));
        }

        public void Remove(T item)
        {
            _references.RemoveAll(r => (r.Target ?? item) == item);
        }

        public void Trim()
        {
            _references.RemoveAll(r => !r.IsAlive);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
