using System;
using System.Collections.Generic;
using TheLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace TheLibrary.Data;

public partial class LibraryDbContext : DbContext
{

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Borrower> Borrowers { get; set; }

    public virtual DbSet<Catalog> Catalogs { get; set; }

    public virtual DbSet<Checkout> Checkouts { get; set; }

    public virtual DbSet<Checkoutdetail> Checkoutdetails { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Librarian> Librarians { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Shelf> Shelves { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=JH3A\\SQLEXPRESS;Initial Catalog=LIBRARY_ADMIN_PROJECT_002;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__AUTHOR__A8398145DCC8BEE7");

            entity.ToTable("AUTHOR");

            entity.Property(e => e.AuthorId).HasColumnName("AUTHOR_ID");
            entity.Property(e => e.AuthorName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AUTHOR_NAME");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__BOOK__6E1C5AE355FD62BD");

            entity.ToTable("BOOK");

            entity.HasIndex(e => e.BookTitle, "IDX_BOOKTITLE");

            entity.Property(e => e.BookId).HasColumnName("BOOKID");
            entity.Property(e => e.AuthorId).HasColumnName("AUTHOR_ID");
            entity.Property(e => e.BookTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BOOK_TITLE");
            entity.Property(e => e.CatalogId).HasColumnName("CATALOG_ID");
            entity.Property(e => e.GenreId).HasColumnName("GENRE_ID");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("IS_AVAILABLE");
            entity.Property(e => e.PublicationDate).HasColumnName("PUBLICATION_DATE");
            entity.Property(e => e.PublisherId).HasColumnName("PUBLISHER_ID");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
            entity.Property(e => e.ShelfId).HasColumnName("SHELF_ID");

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOOK__AUTHOR_ID__20C1E124");

            entity.HasOne(d => d.Catalog).WithMany(p => p.Books)
                .HasForeignKey(d => d.CatalogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOOK__CATALOG_ID__239E4DCF");

            entity.HasOne(d => d.Genre).WithMany(p => p.Books)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOOK__GENRE_ID__21B6055D");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOOK__PUBLISHER___22AA2996");

            entity.HasOne(d => d.Shelf).WithMany(p => p.Books)
                .HasForeignKey(d => d.ShelfId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOOK__SHELF_ID__24927208");
        });

        modelBuilder.Entity<Borrower>(entity =>
        {
            entity.HasKey(e => e.BorrowerId).HasName("PK__BORROWER__DEFFE50E1B204335");

            entity.ToTable("BORROWER");

            entity.Property(e => e.BorrowerId).HasColumnName("BORROWER_ID");
            entity.Property(e => e.BorrowerType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BORROWER_TYPE");
            entity.Property(e => e.BorrowerFname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BORROWER_FNAME");
            entity.Property(e => e.BorrowerFullname)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasComputedColumnSql("(([BORROWER_FNAME]+' ')+[BORROWER_LNAME])", false)
                .HasColumnName("BORROWER_FULLNAME");
            entity.Property(e => e.BorrowerLname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BORROWER_LNAME");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.PhoneNumber).HasColumnName("PHONE_NUMBER");
        });

        modelBuilder.Entity<Catalog>(entity =>
        {
            entity.HasKey(e => e.CatalogId).HasName("PK__CATALOG__DC3C65918D283521");

            entity.ToTable("CATALOG");

            entity.Property(e => e.CatalogId).HasColumnName("CATALOG_ID");
            entity.Property(e => e.CatalogName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CATALOG_NAME");
            entity.Property(e => e.CreationDate).HasColumnName("CREATION_DATE");
        });

        modelBuilder.Entity<Checkout>(entity =>
        {
            entity.HasKey(e => e.CheckoutId).HasName("PK__CHECKOUT__11B144F2C2B7D8BC");

            entity.ToTable("CHECKOUT", tb => tb.HasTrigger("TRG_SetDueDate"));

            entity.Property(e => e.CheckoutId).HasColumnName("CHECKOUT_ID");
            entity.Property(e => e.BorrowerId).HasColumnName("BORROWER_ID");
            entity.Property(e => e.CheckoutDate).HasColumnName("CHECKOUT_DATE");
            entity.Property(e => e.DueDate).HasColumnName("DUE_DATE");
            entity.Property(e => e.LibrarianId).HasColumnName("LIBRARIAN_ID");
            entity.Property(e => e.NotifId).HasColumnName("NOTIF_ID");

            entity.HasOne(d => d.Borrower).WithMany(p => p.Checkouts)
                .HasForeignKey(d => d.BorrowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHECKOUT__BORROW__5441852A");

            entity.HasOne(d => d.Librarian).WithMany(p => p.Checkouts)
                .HasForeignKey(d => d.LibrarianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHECKOUT__LIBRAR__5535A963");

            entity.HasOne(d => d.Notification).WithMany(p => p.Checkouts)
                .HasForeignKey(d => d.NotifId)
                .HasConstraintName("FK__CHECKOUT__NOTIF___5629CD9C");
        });

        modelBuilder.Entity<Checkoutdetail>(entity =>
        {
            entity.HasKey(e => new { e.CheckoutIdDetail, e.CheckoutId, e.Bookid }).HasName("PK__CHECKOUT__9A99209F98C47C6E");

            entity.ToTable("CHECKOUTDETAIL");

            entity.Property(e => e.CheckoutIdDetail)
                .ValueGeneratedOnAdd()
                .HasColumnName("CHECKOUT_ID_DETAIL");
            entity.Property(e => e.CheckoutId).HasColumnName("CHECKOUT_ID");
            entity.Property(e => e.Bookid).HasColumnName("BOOKID");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

            entity.HasOne(d => d.Book).WithMany(p => p.Checkoutdetails)
                .HasForeignKey(d => d.Bookid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHECKOUTD__BOOKI__59FA5E80");

            entity.HasOne(d => d.Checkout).WithMany(p => p.Checkoutdetails)
                .HasForeignKey(d => d.CheckoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHECKOUTD__CHECK__59063A47");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__GENRE__8A48A707EBB35AAB");

            entity.ToTable("GENRE");

            entity.Property(e => e.GenreId).HasColumnName("GENRE_ID");
            entity.Property(e => e.Genre1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GENRE");
        });

        modelBuilder.Entity<Librarian>(entity =>
        {
            entity.HasKey(e => e.LibrarianId).HasName("PK__LIBRARIA__0161F0F77CF72DC9");

            entity.ToTable("LIBRARIAN");

            entity.HasIndex(e => new { e.LibrarianFname, e.LibrarianLname }, "IDX_LIBRARIANNAME");

            entity.Property(e => e.LibrarianId).HasColumnName("LIBRARIAN_ID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.LibrarianFname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("LIBRARIAN_FNAME");
            entity.Property(e => e.LibrarianFullname)
                .HasMaxLength(41)
                .IsUnicode(false)
                .HasComputedColumnSql("(([LIBRARIAN_FNAME]+' ')+[LIBRARIAN_LNAME])", false)
                .HasColumnName("LIBRARIAN_FULLNAME");
            entity.Property(e => e.LibrarianLname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("LIBRARIAN_LNAME");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotifId).HasName("PK__NOTIFICA__52C41D78B4AD7968");

            entity.ToTable("NOTIFICATION");

            entity.HasIndex(e => e.NotifDescription, "IDX_NOTIFDESC");

            entity.Property(e => e.NotifId).HasColumnName("NOTIF_ID");
            entity.Property(e => e.NotifDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOTIF_DESCRIPTION");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__PUBLISHE__7381341FD5EBD94A");

            entity.ToTable("PUBLISHER");

            entity.Property(e => e.PublisherId).HasColumnName("PUBLISHER_ID");
            entity.Property(e => e.PublisherName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PUBLISHER_NAME");
        });

        modelBuilder.Entity<Shelf>(entity =>
        {
            entity.HasKey(e => e.ShelfId).HasName("PK__SHELF__0331CB83851055CB");

            entity.ToTable("SHELF");

            entity.Property(e => e.ShelfId).HasColumnName("SHELF_ID");
            entity.Property(e => e.GenreId).HasColumnName("GENRE_ID");
            entity.Property(e => e.ShelfNumber).HasColumnName("SHELF_NUMBER");

            entity.HasOne(d => d.Genre).WithMany(p => p.Shelves)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SHELF__GENRE_ID__1DE57479");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
