using System;

namespace Inferno
{
    /// <summary>
    /// This converter will allow users to change the way the set functionality is performed in property binding.
    /// </summary>
    public interface ISetMethodBindingConverter
    {
        /// <summary>
        /// Returns a positive integer when this class supports
        /// PerformSet for this particular Type. If the method isn't supported at
        /// all, return a non-positive integer. When multiple implementations
        /// return a positive value, the host will use the one which returns
        /// the highest value. When in doubt, return '2' or '0'.
        /// </summary>
        /// <param name="fromType">The from type to convert from.</param>
        /// <param name="toType">The target type to convert to.</param>
        /// <returns>A positive integer if PerformSet is supported,
        /// zero or a negative value otherwise.</returns>
        int GetAffinityForObjects(Type fromType, Type toType);

        /// <summary>
        /// Convert a given object to the specified type.
        /// </summary>
        /// <param name="toTarget">The target object we are setting to.</param>
        /// <param name="newValue">The value to set on the new object.</param>
        /// <param name="arguments">The arguments required. Used for indexer based values.</param>
        /// <returns>The value that was set.</returns>
        object PerformSet(object toTarget, object newValue, object[] arguments);
    }

    public class NullSetMethodBindingConverter : ISetMethodBindingConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return 0;
        }

        public object PerformSet(object toTarget, object newValue, object[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}