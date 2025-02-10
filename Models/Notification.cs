using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheLibrary.Models;

public partial class Notification
{
    public int NotifId { get; set; }
    [Display(Name = "Notification")]
    public string NotifDescription { get; set; } = null!;

    public virtual ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();
}
