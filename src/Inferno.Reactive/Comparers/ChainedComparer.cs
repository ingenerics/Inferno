using System;
using System.Collections.Generic;

namespace Inferno
{
    internal sealed class ChainedComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> _parent;
        private readonly Comparison<T> _inner;

        public ChainedComparer(IComparer<T> parent, Comparison<T> comparison)
        {
            _parent = parent;
            _inner = comparison ?? throw new ArgumentNullException(nameof(comparison));
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            var parentResult = _parent?.Compare(x, y) ?? 0;

            return parentResult != 0 ? parentResult : _inner(x, y);
        }
    }
}
