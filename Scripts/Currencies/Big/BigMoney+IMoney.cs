using Analytics;
using Dirichlet.Numerics;
using State;
using UnityEngine;

namespace Currencies.Big
{
    public partial class BigMoney
    {
        public override UInt128 Spendable() => Total() - Staged();

        public override MoneyOperation<UInt128> Add(UInt128 amount, bool staged = false)
        {
            if (amount == 0) return Unchanged();
            if (amount < 0) return Fail();
            var oldTotal = Total();
            var oldSpendable = Spendable();
            var newTotal = oldTotal + amount;
            SetTotal(newTotal);
            if (staged) SetStaged(Staged() + amount);
            OnChanged(oldTotal, oldSpendable);
            Debug.Log($"Add {Name} x{amount} [staged={staged}]");
            return Succeed();
        }

        public override MoneyOperation<UInt128> Spend(UInt128 amount, bool staged = false)
        {
            var total = Total();
            var spendable = Spendable();
            if (!CanSpend(amount))
                return Fail();
            var oldTotal = Total();
            var oldSpendable = Spendable();
            if (amount > spendable)
            {
                SetStaged(amount - spendable);
                amount = spendable;
            }
            SetTotal(total - amount);
            OnChanged(oldTotal, oldSpendable);
            Debug.Log($"Spend {Name} x{amount}");
            Tracking.Instance.Track($"spend_{Name}", "amount", amount);
            return Succeed();
        }

        public override MoneyOperation<UInt128> Commit(UInt128 amount)
        {
            var oldTotal = Total();
            var oldSpendable = Spendable();
            if (amount == 0) return Unchanged();
            if (amount < 0) return Fail();
            var staged = Staged();
            if (amount > staged)
                return Fail();
            SetStaged(UInt128.Max(0, staged - amount));
            OnChanged(oldTotal, oldSpendable);
            Debug.Log($"Commit {Name} x{amount}");
            return Succeed();
        }
    }
}