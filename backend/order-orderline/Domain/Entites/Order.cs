using System;
using System.Collections.Generic;

namespace order_orderline.Domain.Entites;

public partial class Order
{
    public int OrderId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public DateOnly OrderDate { get; set; }

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}
