using System;
using System.Collections.Generic;

namespace TheLibrary.Models;

public partial class Catalog
{
    public int CatalogId { get; set; }

    public string CatalogName { get; set; } = null!;

    public DateOnly CreationDate { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
