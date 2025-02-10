using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TheLibrary.Models
{
    public partial class Book
    {
        public int BookId { get; set; }

        [Display(Name = "Book Title")]
        public string? BookTitle { get; set; }

        [Display(Name = "Publication Date")]
        public DateOnly? PublicationDate { get; set; }

        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        [Display(Name = "Publisher")]
        public int PublisherId { get; set; }

        [Display(Name = "Catalog")]
        public int CatalogId { get; set; }

        [Display(Name = "Shelf #")]
        public int ShelfId { get; set; }

        [Display(Name = "Stocks")]
        public int Quantity { get; set; }

        [Display(Name = "Availability")]
        public bool IsAvailable { get; set; }

        // BorrowedCount property
        [Display(Name = "Borrowed Books")]
        public int BorrowedCount
        {
            get
            {
                return Checkoutdetails.Count(cd => !cd.IsReturned);
            }
        }

        public virtual Author? Author { get; set; }
        public virtual Catalog? Catalog { get; set; }
        public virtual ICollection<Checkoutdetail> Checkoutdetails { get; set; } = new List<Checkoutdetail>();
        public virtual Genre? Genre { get; set; }
        public virtual Publisher? Publisher { get; set; }
        public virtual Shelf? Shelf { get; set; }
    }
}
