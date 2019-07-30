using Dirichlet.Numerics;

namespace Currencies
{
    public struct CurrencyEvent
    {
        public readonly Currency currency;
        public readonly UInt128 from;
        public readonly UInt128 to;

        public CurrencyEvent(Currency currency, UInt128 from, UInt128 to)
        {
            this.currency = currency;
            this.from = from;
            this.to = to;
        }
    }
}