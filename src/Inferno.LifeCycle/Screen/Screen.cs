namespace Inferno
{
    public class Screen : ReactiveObject, IViewAware
    {
        public Screen()
        {
            View = new ViewSink();
        }

        #region IViewAware

        /// <summary>
        /// A sink where the view can post messages.
        /// </summary>
        public ViewSink View { get; }

        #endregion IViewAware
    }
}
