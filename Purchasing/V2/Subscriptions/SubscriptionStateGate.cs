using System;
using State.Active;
using Stencil.Economy.Purchasing.Subscriptions;

namespace Purchasing.V2.Subscriptions
{
    public class SubscriptionStateGate : ActiveGate
    {
        public override void Register(ActiveManager manager)
        {
            base.Register(manager);
            StencilSubscriptions.OnStateChanged += _OnState;
        }

        public override void Unregister()
        {
            base.Unregister();
            StencilSubscriptions.OnStateChanged -= _OnState;
        }

        private void _OnState(object sender, EventArgs e)
        {
            RequestCheck();
        }

        public override bool? Check()
        {
            return StencilSubscriptions.IsSubscribed;
        }
    }
}