using System;
using QuantConnect.Securities;

namespace QuantConnect.Orders.Fees;

/// <summary>
/// An order fee that can run an action when fee is applied to portfolio
/// </summary>
public class ActionableOrderFee : OrderFee
{
    private readonly Action<OrderEvent> _applyAction;

    public ActionableOrderFee(Action<OrderEvent> applyAction, CashAmount amount)
        : base(amount)
    {
        _applyAction = applyAction ?? throw new ArgumentNullException(nameof(applyAction));
    }

    public override void ApplyToPortfolio(SecurityPortfolioManager portfolio, OrderEvent fill)
    {
        _applyAction(fill);
        base.ApplyToPortfolio(portfolio, fill);
    }
}
