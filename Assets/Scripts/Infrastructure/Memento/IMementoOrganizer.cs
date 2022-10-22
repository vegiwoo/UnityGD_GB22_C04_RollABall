
// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    /// <summary>
    /// It can take snapshots of its state, as well as reproduce the past state if a ready-made snapshot is fed into it.
    /// </summary>
    /// <remarks>Used in 'Memento' pattern.</remarks>>
    public interface IMementoOrganizer<TState>
    {
        /// <summary>
        /// Stores state of organizer.
        /// </summary>
        public TState State { get; set; }

        /// <summary>
        /// Stores current state of organizer inside the snapshot.
        /// </summary>
        /// <returns></returns>
        IMemento<TState> Save();

        /// <summary>
        /// Restores the organizer state from a snapshot.
        /// </summary>
        /// <param name="memento">State snapshot</param>
        public void Load(IMemento<TState> memento);
    }
}