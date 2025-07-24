using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;

namespace BusinessObjects.DBContext;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Customerform> Customerforms { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Lawform> Lawforms { get; set; }

    public virtual DbSet<Lawtype> Lawtypes { get; set; }

    public virtual DbSet<Lawyer> Lawyers { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Usercredit> Usercredits { get; set; }

//     public static string GetConnectionString(string connectionStringName)
{
    var config = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .Build();

    string connectionString = config.GetConnectionString(connectionStringName);
    return connectionString;
}

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseNpgsql(GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Appointmentid).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.Property(e => e.Appointmentid).HasColumnName("appointmentid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Lawtypeid).HasColumnName("lawtypeid");
            entity.Property(e => e.Lawyerid).HasColumnName("lawyerid");
            entity.Property(e => e.Meetinglink)
                .HasMaxLength(500)
                .HasColumnName("meetinglink");
            entity.Property(e => e.Scheduledate).HasColumnName("scheduledate");
            entity.Property(e => e.Scheduletime).HasColumnName("scheduletime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Totalamount)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("totalamount");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Lawtype).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Lawtypeid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("appointments_lawtypeid_fkey");

            entity.HasOne(d => d.Lawyer).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Lawyerid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("appointments_lawyerid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("appointments_userid_fkey");
        });

        modelBuilder.Entity<Customerform>(entity =>
        {
            entity.HasKey(e => e.Customerformid).HasName("customerforms_pkey");

            entity.ToTable("customerforms");

            entity.Property(e => e.Customerformid).HasColumnName("customerformid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Formdata)
                .HasColumnType("jsonb")
                .HasColumnName("formdata");
            entity.Property(e => e.Lawformid).HasColumnName("lawformid");
            entity.Property(e => e.Linkform)
                .HasMaxLength(500)
                .HasColumnName("linkform");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Draft'::text")
                .HasColumnName("status");
            entity.Property(e => e.Totalamount)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("totalamount");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Lawform).WithMany(p => p.Customerforms)
                .HasForeignKey(d => d.Lawformid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("customerforms_lawformid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Customerforms)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("customerforms_userid_fkey");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Feedbackid).HasName("feedbacks_pkey");

            entity.ToTable("feedbacks");

            entity.Property(e => e.Feedbackid).HasColumnName("feedbackid");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("feedbacks_userid_fkey");
        });

        modelBuilder.Entity<Lawform>(entity =>
        {
            entity.HasKey(e => e.Lawformid).HasName("lawform_pkey");

            entity.ToTable("lawform");

            entity.Property(e => e.Lawformid).HasColumnName("lawformid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Formpath)
                .HasMaxLength(500)
                .HasColumnName("formpath");
            entity.Property(e => e.Lawtypeid).HasColumnName("lawtypeid");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("price");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Lawtype).WithMany(p => p.Lawforms)
                .HasForeignKey(d => d.Lawtypeid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("lawform_lawtypeid_fkey");
        });

        modelBuilder.Entity<Lawtype>(entity =>
        {
            entity.HasKey(e => e.Lawtypeid).HasName("lawtypes_pkey");

            entity.ToTable("lawtypes");

            entity.HasIndex(e => e.Lawtype1, "lawtypes_lawtype_key").IsUnique();

            entity.Property(e => e.Lawtypeid).HasColumnName("lawtypeid");
            entity.Property(e => e.Lawtype1).HasColumnName("lawtype");
        });

        modelBuilder.Entity<Lawyer>(entity =>
        {
            entity.HasKey(e => e.Lawyerid).HasName("lawyers_pkey");

            entity.ToTable("lawyers");

            entity.Property(e => e.Lawyerid).HasColumnName("lawyerid");
            entity.Property(e => e.Consultationfee)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("consultationfee");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Experience)
                .HasDefaultValue(0)
                .HasColumnName("experience");
            entity.Property(e => e.Qualification).HasColumnName("qualification");
            entity.Property(e => e.Rating)
                .HasPrecision(3, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("rating");
            entity.Property(e => e.Specialties).HasColumnName("specialties");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Lawyers)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("lawyers_userid_fkey");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.Packageid).HasName("packages_pkey");

            entity.ToTable("packages");

            entity.Property(e => e.Packageid).HasColumnName("packageid");
            entity.Property(e => e.Bookingcount)
                .HasDefaultValue(0)
                .HasColumnName("bookingcount");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Lawformcount)
                .HasDefaultValue(0)
                .HasColumnName("lawformcount");
            entity.Property(e => e.Packagename)
                .HasMaxLength(255)
                .HasColumnName("packagename");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("price");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("payment_pkey");

            entity.ToTable("payment");

            entity.HasIndex(e => e.Appointmentid, "payment_appointmentid_key").IsUnique();

            entity.HasIndex(e => e.Customerformid, "payment_customerformid_key").IsUnique();

            entity.HasIndex(e => e.Packageid, "payment_packageid_key").IsUnique();

            entity.HasIndex(e => e.Transactionid, "payment_transactionid_key").IsUnique();

            entity.Property(e => e.Paymentid).HasColumnName("paymentid");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Appointmentid).HasColumnName("appointmentid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Customerformid).HasColumnName("customerformid");
            entity.Property(e => e.Packageid).HasColumnName("packageid");
            entity.Property(e => e.Paymentdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paymentdate");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Transactionid)
                .HasMaxLength(255)
                .HasColumnName("transactionid");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Appointment).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.Appointmentid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payment_appointmentid_fkey");

            entity.HasOne(d => d.Customerform).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.Customerformid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payment_customerformid_fkey");

            entity.HasOne(d => d.Package).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.Packageid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payment_packageid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("payment_userid_fkey");
        });

        modelBuilder.Entity<Refreshtoken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refreshtokens_pkey");

            entity.ToTable("refreshtokens");

            entity.HasIndex(e => e.Token, "refreshtokens_token_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Addeddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("addeddate");
            entity.Property(e => e.Expirydate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expirydate");
            entity.Property(e => e.Isrevoked)
                .HasDefaultValue(false)
                .HasColumnName("isrevoked");
            entity.Property(e => e.Isused)
                .HasDefaultValue(false)
                .HasColumnName("isused");
            entity.Property(e => e.Jwtid)
                .HasMaxLength(255)
                .HasColumnName("jwtid");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Refreshtokens)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("refreshtokens_userid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Avatar)
                .HasMaxLength(500)
                .HasColumnName("avatar");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Customer'::character varying")
                .HasColumnName("role");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
        });

        modelBuilder.Entity<Usercredit>(entity =>
        {
            entity.HasKey(e => e.Usercreditid).HasName("usercredit_pkey");

            entity.ToTable("usercredit");

            entity.Property(e => e.Usercreditid).HasColumnName("usercreditid");
            entity.Property(e => e.Bookingremaining)
                .HasDefaultValue(0)
                .HasColumnName("bookingremaining");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Lawformremaining)
                .HasDefaultValue(0)
                .HasColumnName("lawformremaining");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Usercredits)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usercredit_userid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
