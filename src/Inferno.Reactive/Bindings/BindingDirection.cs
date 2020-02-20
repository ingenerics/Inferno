namespace Inferno
{
    /// <summary>
    /// The type of binding that the ReactiveBinding represents.
    /// </summary>
    public enum BindingDirection
    {
        /// <summary>The binding is updated only one way from the ViewModel.</summary>
        OneWay,

        /// <summary>The binding is updated from both the View and the ViewModel.</summary>
        TwoWay,

        /// <summary>The binding is updated asynchronously one way from the ViewModel.</summary>
        AsyncOneWay,
    }
}