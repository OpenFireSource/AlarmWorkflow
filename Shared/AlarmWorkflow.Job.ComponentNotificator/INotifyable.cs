using System;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Job.ComponentNotificator
{
    /// <summary>
    /// Defines a method that is called by the <see cref="T:ComponentNotificatorJob"/>.
    /// See documentation on how to use this interface.
    /// </summary>
    /// <remarks><para>In order for this interface to get called, you need to decorate the implementing type with:</para>
    /// <para>[Export("YOUR_ALIAS", typeof(INotifyable))]</para>
    /// <para>It will then get initialized automatically.</para></remarks>
    public interface INotifyable : IDisposable
    {
        /// <summary>
        /// Performs custom initialization work for this instance.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Called by the notificator job on each registered component.
        /// </summary>
        /// <param name="operation"></param>
        void Notify(Operation operation);
    }
}
