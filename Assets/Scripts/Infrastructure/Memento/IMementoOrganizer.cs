
// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    /// <summary>
    /// It can take snapshots of its state, as well as reproduce the past state if a ready-made snapshot is fed into it.
    /// </summary>
    /// <remarks>Used in 'Memento' pattern.</remarks>>
    public interface IMementoOrganizer<S>
    {
        /// <summary>
        /// Stores state of organizer.
        /// </summary>
        S State { get; set; }

        /// <summary>
        /// Creates/updates Organizer state.
        /// </summary>
        /// <returns>New state.</returns>
        S MakeState();
        
        /// <summary>
        /// Stores current state of organizer inside the snapshot.
        /// </summary>
        /// <returns></returns>
        IMemento<S> Save();

        /// <summary>
        /// Restores the organizer state from a snapshot.
        /// </summary>
        /// <param name="memento">State snapshot</param>
        public void Load(IMemento<S> memento);
    }
}