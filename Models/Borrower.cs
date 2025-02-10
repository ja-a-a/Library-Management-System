using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheLibrary.Models;

namespace TheLibrary.Models;

public partial class Borrower
{
    [Display(Name = "ID")]
    public int BorrowerId { get; set; }

    [Required(ErrorMessage = "Borrower Type is required.")]
    [Display(Name = "Type")]
    public string BorrowerType { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [Display(Name = "First Name")]
    public string BorrowerFname { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [Display(Name = "Last Name")]
    public string BorrowerLname { get; set; }

    [Display(Name = "Borrower")]
    public string BorrowerFullname { get; set; } 


    [Required(ErrorMessage = "Contact number is required.")]
    [Display(Name = "Contact No.")]
    public int PhoneNumber { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }

    public virtual ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();
}
    