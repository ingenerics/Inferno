using System;

namespace Inferno
{
    /// <summary>
    /// Implement this interface to override how Inferno determines when a
    /// View is added or removed from the visual tree (ie loaded, resp. unloaded).
    /// This is usually only used when porting to a new UI framework.
    /// </summary>
    public interface ILoadedForViewFetcher
    {
        /// <summary>
        /// Determines the priority by which the ILoadedForViewFetcher will be able to process the view type.
        /// 0 means it cannot act as a sync for the View, value larger than 0 indicates it can act as a sync for the View.
        /// The class derived off ILoadedForViewFetcher which returns the highest affinity value will be used as fetcher.
        /// </summary>
        /// <param name="view">The type for the View.</param>
        /// <returns>The affinity value which is equal to 0 or above.</returns>
        int GetAffinityForView(Type view);

        /// <summary>
        /// Gets a Observable which will indicate the if the View is part of the visual tree.
        /// This is called after the GetAffinityForView method.
        /// </summary>
        /// <param name="view">The view to get the observable for.</param>
        /// <returns>A Observable which ticks true if the element was successfully loaded.</returns>
        IObservable<bool> GetLoadedForView(object view);
    }
}
