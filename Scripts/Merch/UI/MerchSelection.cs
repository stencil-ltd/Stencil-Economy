using System;
using Merch.System;

namespace Merch.UI
{
    public class MerchSelection : MerchItemActivation
    {
        private void OnEnable()
        {
            MerchSystem.Instance.OnChange += OnChange;
            Refresh();
        }

        private void OnDisable()
        {
            MerchSystem.Instance.OnChange -= OnChange;
        }

        private void OnChange(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            var selection = MerchSystem.Instance.Selected;
            if (selection)
                SetResult(new MerchResult(selection, MerchSystem.Instance.GetState(selection)));
            else
                ClearResult();
        }
    }
}