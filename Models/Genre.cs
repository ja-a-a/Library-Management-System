using System;
using System.Collections.Generic;

namespace TheLibrary.Models;

public partial class Genre
{
    public int GenreId { get; set; }

    public string? Genre1 { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
}
