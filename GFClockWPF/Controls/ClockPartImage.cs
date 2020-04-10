using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace GFClock.Controls
{
    // A custom class for the clock face and hands.
    public class ClockPartImage : Image
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ClockHandAutomationPeer(this);
        }
    }

    public class ClockHandAutomationPeer : ImageAutomationPeer
    {
        public ClockHandAutomationPeer(Image owner)
            : base(owner)
        {
        }

        // Remove the UIA elements representing the clock hands from the 
        // UIA views which are meant to contain things of interest to the 
        // customer. The customer doesn't need to encounter the hands,
        // because the full time is accessible through the clock element.
        protected override bool IsControlElementCore()
        {
            return false;
        }

        protected override bool IsContentElementCore()
        {
            return false;
        }
    }
}
