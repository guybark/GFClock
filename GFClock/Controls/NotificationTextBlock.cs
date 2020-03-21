using System;
using System.Runtime.InteropServices;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace GFClock.Controls
{
    // This demo snippet was run after a NotificationTextBlock had been 
    // added to the app's XAML using <local:NotificationTextBlock>.
    internal class NotificationTextBlock : TextBlock
    {
        // This control's AutomationPeer is the object that actually raises the UIA Notification event.
        private NotificationTextBlockAutomationPeer _peer;

        // Assume the UIA Notification event is available until we learn otherwise.
        // If we learn that the UIA Notification event is not available, no instance 
        // of the NotificationTextBlock should attempt to raise it.
        static private bool _notificationEventAvailable = true;

        public bool NotificationEventAvailable { get => _notificationEventAvailable; set => _notificationEventAvailable = value; }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            this._peer = new NotificationTextBlockAutomationPeer(this);

            return this._peer;
        }

        public void RaiseNotificationEvent(string notificationText, string notificationId)
        {
            // Only attempt to raise the event if we already have an AutomationPeer.
            if (this._peer != null)
            {
                this._peer.RaiseNotificationEvent(notificationText, notificationId);
            }
        }

        private class NotificationTextBlockAutomationPeer : TextBlockAutomationPeer
        {
            private NotificationTextBlock _notificationTextBlock;

            // The UIA Notification event requires the IRawElementProviderSimple 
            // associated with this AutomationPeer.
            private IRawElementProviderSimple _reps;

            public NotificationTextBlockAutomationPeer(NotificationTextBlock owner) : base(owner)
            {
                this._notificationTextBlock = owner;
            }

            public void RaiseNotificationEvent(string notificationText, string notificationId)
            {
                // If we already know that the UIA Notification is not available, do not 
                // attempt to raise it.
                if (this._notificationTextBlock.NotificationEventAvailable)
                {
                    // Get the IRawElementProviderSimple for this AutomationPeer if we don't 
                    // have it already.
                    if (this._reps == null)
                    {
                        AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this._notificationTextBlock);
                        if (peer != null)
                        {
                            this._reps = ProviderFromPeer(peer);
                        }
                    }

                    if (this._reps != null)
                    {
                        try
                        {
                            // IMPORTANT: The NotificationKind and NotificationProcessing values shown 
                            // here are sample values for the snippet. You should use whatever values 
                            // are appropriate for your scenarios.

                            NativeMethods.UiaRaiseNotificationEvent(
                                this._reps,
                                /* SAMPLE */ NativeMethods.NotificationKind.NotificationKind_ActionCompleted,
                                /* SAMPLE */ NativeMethods.NotificationProcessing.NotificationProcessing_MostRecent,
                                notificationText,
                                notificationId);
                        }
                        catch (EntryPointNotFoundException)
                        {
                            // The UIA Notification event is not available, so don't attempt
                            // to raise it again.
                            _notificationTextBlock.NotificationEventAvailable = false;
                        }
                    }
                }
            }
        }
    }

    internal class NativeMethods
    {
        // Documentation on NotificationProcessing is at:
        // https://docs.microsoft.com/en-us/windows/win32/api/uiautomationcore/ne-uiautomationcore-notificationprocessing
        public enum NotificationProcessing
        {
            NotificationProcessing_ImportantAll,
            NotificationProcessing_ImportantMostRecent,
            NotificationProcessing_All,
            NotificationProcessing_MostRecent,
            NotificationProcessing_CurrentThenMostRecent
        };

        // Documentation on NotificationKind is at:
        // https://docs.microsoft.com/en-us/windows/win32/api/uiautomationcore/ne-uiautomationcore-notificationkind
        public enum NotificationKind
        {
            NotificationKind_ItemAdded,
            NotificationKind_ItemRemoved,
            NotificationKind_ActionCompleted,
            NotificationKind_ActionAborted,
            NotificationKind_Other
        };

        // Documentation on UiaRaiseNotificationEvent is at:
        // https://docs.microsoft.com/en-us/windows/win32/api/uiautomationcoreapi/nf-uiautomationcoreapi-uiaraisenotificationevent
        [DllImport("UIAutomationCore.dll", EntryPoint = "UiaRaiseNotificationEvent", CharSet = CharSet.Unicode)]
        public static extern int UiaRaiseNotificationEvent(
            IRawElementProviderSimple provider,
            NotificationKind notificationKind,
            NotificationProcessing notificationProcessing,
            string notificationText,
            string notificationId);
    }
}
