using System;
using System.Runtime.Serialization;
using Common;
using Scripts.RemoteConfig;
using UnityEngine;

namespace Currencies
{
    public interface IMoney<T> : INameable, IRemoteId where T : IFormattable, IComparable, IComparable<T>, IEquatable<T>
    {    
        /** Events **/
        event EventHandler<IMoney<T>> OnTotalChanged;
        event EventHandler<IMoney<T>> OnSpendableChanged;
        
        /** Getters **/
        T Total();
        T Spendable();
        T Staged();
        T Lifetime();
        
        /** Operations **/
        MoneyOperation<T> Add(T amount, bool staged);
        MoneyOperation<T> Spend(T amount, bool staged);
        MoneyOperation<T> Commit(T amount);

        /** Utility **/
        bool CanSpend(T amount);
        void Clear();
        void Save();
        
        /** UI **/
        Sprite Sprite();
    }
}