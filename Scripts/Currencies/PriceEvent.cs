using System;
using UnityEngine.Events;

namespace Currencies
{
    [Serializable]
    public class PriceEvent : UnityEvent<Price>
    {
    }
}