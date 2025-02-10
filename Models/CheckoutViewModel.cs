using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheLibrary.Models
{
    public class CheckoutViewModel
    {
        [Display(Name = "Checkout ID")]
        public int CheckoutId { get; set; }

        [Display(Name = "Borrower")]
        public int BorrowerId { get; set; }

        public List<int> SelectedBookIds { get; set; } = new List<int>();

        public List<Book> AllBooks { get; set; } = new List<Book>();

        [Display(Name = "Borrower Full Name")]
        public string BorrowerFullname { get; set; } = null!;

        [Display(Name = "Type")]
        public string BorrowerType { get; set; } = null!;

        [Display(Name = "Checkout Date")]
        [DataType(DataType.Date)]
        public DateTime CheckoutDate { get; set; } = DateTime.Today;

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);
        [Display(Name = "Librarian")]
        public int LibrarianId { get; set; }

        [Display(Name = "Notification")]
        public int NotifId { get; set; }

        public Checkout Checkout { get; set; } = null!;

        public Checkoutdetail Checkoutdetail { get; set; } = null!;
    }
}
