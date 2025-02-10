using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheLibrary.Models
{
    public partial class Checkout
    {
        [Display(Name = "Checkout Id")]
        public int CheckoutId { get; set; }

        [Required(ErrorMessage = "Borrower is required.")]
        [Display(Name = "Borrower")]
        public int BorrowerId { get; set; }

        [Required(ErrorMessage = "Librarian is required.")]
        [Display(Name = "Librarian")]
        public int LibrarianId { get; set; }

        [Required(ErrorMessage = "Checkout Date is required.")]
        [Display(Name = "Checkout Date")]
        [DataType(DataType.Date)]
        public DateOnly CheckoutDate { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateOnly? DueDate { get; set; }

        [Display(Name = "Notification")]
        public int? NotifId { get; set; }

        public virtual Borrower? Borrower { get; set; } = null;

        public virtual ICollection<Checkoutdetail> Checkoutdetails { get; set; } = new List<Checkoutdetail>();

        public virtual Librarian? Librarian { get; set; }

        public virtual Notification? Notification { get; set; }
    }
}
