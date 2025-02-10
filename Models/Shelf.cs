using System;
using System.Collections.Generic;

namespace TheLibrary.Models;

public partial class Shelf
{
    public int ShelfId { get; set; }

    public int ShelfNumber { get; set; }

    public int GenreId { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual Genre Genre { get; set; } = null!;
}
