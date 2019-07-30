namespace Currencies
{
    public struct CurrencyOperation
    {
        public readonly Currency Resource;
        public readonly bool Success;
        public readonly bool Changed;

        public static CurrencyOperation Fail(Currency type) => new CurrencyOperation(type, false, false);
        public static CurrencyOperation Unchanged(Currency type) => new CurrencyOperation(type, true, false);
        public static CurrencyOperation Succeed(Currency type) => new CurrencyOperation(type, true, true);

        public CurrencyOperation(Currency resource, bool success, bool changed)
        {
            Resource = resource;
            Success = success;
            Changed = changed;
        }

        public bool AndSave()
        {
            if (!Success) return false;
            Resource.Save();
            return true;
        }
    }
}