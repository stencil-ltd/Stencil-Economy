using System;

namespace Currencies
{
    public struct MoneyOperation<T> where T : IFormattable, IComparable, IComparable<T>, IEquatable<T>
    {
        public readonly IMoney<T> money;
        public readonly bool success;
        public readonly bool changed;

        public static MoneyOperation<T> Fail(IMoney<T> type) =>
            new MoneyOperation<T>(type, false, false);

        public static MoneyOperation<T> Unchanged(IMoney<T> type) =>
            new MoneyOperation<T>(type, true, false);

        public static MoneyOperation<T> Succeed(IMoney<T> type) =>
            new MoneyOperation<T>(type, true, true);

        public MoneyOperation(IMoney<T> money, bool success, bool changed)
        {
            this.money = money;
            this.success = success;
            this.changed = changed;
        }

        public bool AndSave()
        {
            if (!success) return false;
            money.Save();
            return true;
        }
    }
}