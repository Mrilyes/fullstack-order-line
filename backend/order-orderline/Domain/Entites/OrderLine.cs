using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace order_orderline.Domain.Entites;

public partial class OrderLine
{
    [Key]
    public int OrderLineId { get; set; }  // Primary key

    public int? OrderId { get; set; }     // Foreign key to Order (nullable)

    public int? ArticleId { get; set; }   // Foreign key to Article (nullable)

    public string? ProductName { get; set; }

    public int Quantity { get; set; }     // Quantity of the article in the order line

    public decimal Price { get; set; } = 0.0m; // Price of the article in the order line

    // Navigation properties

    [ForeignKey("ArticleId")]
    public virtual Article? Article { get; set; }

    [ForeignKey("OrderId")]
    public virtual Order? Order { get; set; }
}
