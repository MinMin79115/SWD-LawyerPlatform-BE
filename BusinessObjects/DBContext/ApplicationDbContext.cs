using System;
using System.Collections.Generic;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Duration> Durations { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Lawform> Lawforms { get; set; }

    public virtual DbSet<Lawtype> Lawtypes { get; set; }

    public virtual DbSet<Lawyer> Lawyers { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }
    
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Servicestype> Servicestypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Usercredit> Usercredits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=LawyerPlatform;Username=postgres;Password=791156");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("appointment_status", new[] { "Pending", "Confirmed", "Completed", "Cancelled" })
            .HasPostgresEnum("duration_type", new[] { "30Minutes", "60Minutes", "90Minutes", "120Minutes" })
            .HasPostgresEnum("form_status", new[] { "Draft", "Submitted", "Processing", "Completed", "Cancelled" })
            .HasPostgresEnum("law_type", new[] { "RealEstateLaw", "CriminalLaw", "LaborLaw", "EnterpriseLaw" })
            .HasPostgresEnum("payment_status", new[] { "Pending", "Completed", "Failed", "Refunded" })
            .HasPostgresEnum("service_type", new[] { "BookConsultant", "LawForm" })
            .HasPostgresEnum("user_role", new[] { "Customer", "Lawyer", "Admin" });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Appointmentid).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.HasIndex(e => e.Scheduledate, "idx_appointments_date");

            entity.HasIndex(e => e.Lawyerid, "idx_appointments_lawyerid");

            entity.HasIndex(e => e.Userid, "idx_appointments_userid");

            entity.Property(e => e.Appointmentid).HasColumnName("appointmentid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Lawyerid).HasColumnName("lawyerid");
            entity.Property(e => e.Meetinglink)
                .HasMaxLength(500)
                .HasColumnName("meetinglink");
            entity.Property(e => e.Scheduledate).HasColumnName("scheduledate");
            entity.Property(e => e.Scheduletime).HasColumnName("scheduletime");
            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("appointment_status")
                .HasDefaultValue(AppointmentStatus.Pending)
                .HasConversion<string>();
            entity.Property(e => e.Totalamount)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("totalamount");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Lawyer).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Lawyerid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("appointments_lawyerid_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("appointments_serviceid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("appointments_userid_fkey");
        });

        modelBuilder.Entity<Customerform>(entity =>
        {
            entity.HasKey(e => e.Customerformid).HasName("customerforms_pkey");

            entity.ToTable("customerforms");

            entity.HasIndex(e => e.Userid, "idx_customerforms_userid");

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
                .HasColumnName("status")
                .HasColumnType("form_status")
                .HasDefaultValue(FormStatus.Draft)
                .HasConversion<string>();
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

        modelBuilder.Entity<Duration>(entity =>
        {
            entity.HasKey(e => e.Durationid).HasName("durations_pkey");

            entity.ToTable("durations");

            entity.Property(e => e.Durationid).HasColumnName("durationid");
            entity.Property(e => e.Duration)
                .HasColumnName("duration")
                .HasColumnType("duration_type")
                .HasConversion<string>();
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
            entity.Property(e => e.Servicestypeid).HasColumnName("servicestypeid");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Lawtype).WithMany(p => p.Lawforms)
                .HasForeignKey(d => d.Lawtypeid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("lawform_lawtypeid_fkey");

            entity.HasOne(d => d.Servicestype).WithMany(p => p.Lawforms)
                .HasForeignKey(d => d.Servicestypeid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("lawform_servicestypeid_fkey");
        });

        modelBuilder.Entity<Lawtype>(entity =>
        {
            entity.HasKey(e => e.Lawtypeid).HasName("lawtypes_pkey");

            entity.ToTable("lawtypes");

            entity.Property(e => e.Lawtypeid).HasColumnName("lawtypeid");
            entity.Property(e => e.LawType)
                .HasColumnName("lawtype")
                .HasColumnType("law_type")
                .HasConversion<string>();
        });

        modelBuilder.Entity<Lawyer>(entity =>
        {
            entity.HasKey(e => e.Lawyerid).HasName("lawyers_pkey");

            entity.ToTable("lawyers");

            entity.HasIndex(e => e.Rating, "idx_lawyers_rating");

            entity.HasIndex(e => e.Userid, "idx_lawyers_userid");

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

            entity.HasIndex(e => e.Transactionid, "idx_payment_transactionid");

            entity.HasIndex(e => e.Userid, "idx_payment_userid");

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
                .HasColumnName("status")
                .HasColumnType("payment_status")
                .HasDefaultValue(PaymentStatus.Pending)
                .HasConversion<string>();
            entity.Property(e => e.Transactionid)
                .HasMaxLength(255)
                .HasColumnName("transactionid");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Appointmentid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payment_appointmentid_fkey");

            entity.HasOne(d => d.Customerform).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Customerformid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payment_customerformid_fkey");

            entity.HasOne(d => d.Package).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Packageid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payment_packageid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("payment_userid_fkey");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refreshtokens_pkey");

            entity.ToTable("refreshtokens");

            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .HasColumnName("token");
            entity.Property(e => e.JwtId)
                .HasMaxLength(500)
                .HasColumnName("jwtid");
            entity.Property(e => e.IsUsed).HasColumnName("isused");
            entity.Property(e => e.IsRevoked).HasColumnName("isrevoked");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.ExpiryDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expirydate");
            entity.Property(e => e.UserId).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("refreshtokens_userid_fkey");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Serviceid).HasName("services_pkey");

            entity.ToTable("services");

            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Durationid).HasColumnName("durationid");
            entity.Property(e => e.Lawtypeid).HasColumnName("lawtypeid");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("price");
            entity.Property(e => e.Servicestypeid).HasColumnName("servicestypeid");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Duration).WithMany(p => p.Services)
                .HasForeignKey(d => d.Durationid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("services_durationid_fkey");

            entity.HasOne(d => d.Lawtype).WithMany(p => p.Services)
                .HasForeignKey(d => d.Lawtypeid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("services_lawtypeid_fkey");

            entity.HasOne(d => d.Servicestype).WithMany(p => p.Services)
                .HasForeignKey(d => d.Servicestypeid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("services_servicestypeid_fkey");
        });

        modelBuilder.Entity<Servicestype>(entity =>
        {
            entity.HasKey(e => e.Servicetypeid).HasName("servicestypes_pkey");

            entity.ToTable("servicestypes");

            entity.Property(e => e.Servicetypeid).HasColumnName("servicetypeid");
            entity.Property(e => e.ServicesType)
                .HasColumnName("servicestype")
                .HasColumnType("service_type")
                .HasConversion<string>();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "idx_users_email");

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
                .HasColumnName("role")
                .HasColumnType("user_role")
                .HasDefaultValue(UserRole.Customer)
                .HasConversion<string>();
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
        });

        modelBuilder.Entity<Usercredit>(entity =>
        {
            entity.HasKey(e => e.Usercreditid).HasName("usercredit_pkey");

            entity.ToTable("usercredit");

            entity.HasIndex(e => e.Userid, "idx_usercredit_userid");

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
