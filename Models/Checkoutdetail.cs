using System;
using System.Collections.Generic;

namespace TheLibrary.Models;

public partial class Checkoutdetail
{
    public int CheckoutIdDetail { get; set; }

    public int CheckoutId { get; set; }

    public int Bookid { get; set; }

    public bool IsReturned => false;

    public int Quantity { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Checkout Checkout { get; set; } = null!;
}
