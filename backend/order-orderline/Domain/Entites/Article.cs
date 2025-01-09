using System;
using System.Collections.Generic;

namespace order_orderline.Domain.Entites;

public partial class Article
{
    public int ArticleId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}
