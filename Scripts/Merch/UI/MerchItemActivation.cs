using Merch.System;
using UnityEngine;

namespace Merch.UI
{
    public class MerchItemActivation : MonoBehaviour
    {
        [Header("Data")]
        public MerchResult Result;
        
        [Header("State")]
        public GameObject[] Selected = { };
        public GameObject[] Unselected = { };
        public GameObject[] Acquired = { };
        public GameObject[] Unacquired = { };
        public GameObject[] Unlocked = { };
        public GameObject[] Locked = { };
        public GameObject[] Equipped = { };
        public GameObject[] Unequipped = { };
        public GameObject[] Affordable = { };
        public GameObject[] Unaffordable = { };

        private void Start()
        {
            if (Result.Item != null) 
                SetResult(Result);
        }

        public void SetResult(MerchResult result)
        {
            Result = result;
            ActivateStateUi();
        }

        public void ClearResult()
        {
            Result = default(MerchResult);
            ActivateAll(false, Selected, Unselected, Acquired, Unacquired, Locked, Unlocked, Equipped, Unequipped, Affordable, Unaffordable);
        }
        
        private void ActivateStateUi()
        {
            Activate(Selected, Unselected, Result.State.Selected);
            Activate(Acquired, Unacquired, Result.State.Acquired);
            Activate(Locked, Unlocked, Result.State.Locked);
            Activate(Equipped, Unequipped, Result.State.Equipped);
            Activate(Affordable, Unaffordable, !Result.State.Acquired && MerchSystem.Instance.CanPurchase(Result.Item, Result.State.MainPrice.Currency));
        }
        
        private void Activate(GameObject[] positive, GameObject[] negative, bool value)
        {
            foreach (var o in positive)
                o.SetActive(value);
            foreach (var o in negative)
                o.SetActive(!value);
        }

        private void ActivateAll(bool value, params GameObject[][] objs)
        {
            foreach (var gameObjects in objs)
                foreach (var o in gameObjects)
                    o.SetActive(value);
        }
    }
}