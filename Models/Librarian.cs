using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheLibrary.Models;

public partial class Librarian
{
    public int LibrarianId { get; set; }

    public string LibrarianFname { get; set; } = null!;

    public string LibrarianLname { get; set; } = null!;
    [Display(Name = "Librarian")]
    public string LibrarianFullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();
}
