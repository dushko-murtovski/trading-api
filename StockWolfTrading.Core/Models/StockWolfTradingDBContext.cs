using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace StockWolfTrading.Core.Models
{
    public partial class StockWolfTradingDBContext : DbContext
    {
        private AppSettings appSettings { get; set; }

        protected readonly IConfiguration Configuration;
        public IConfiguration GetConfiguration { get { return Configuration; } }

        public StockWolfTradingDBContext(AppSettings settings)
        {
            appSettings = settings;
        }

        public StockWolfTradingDBContext(DbContextOptions<StockWolfTradingDBContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        public virtual DbSet<Algorithm> Algorithm { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<OrderDetailsTicker> OrderDetailsTicker { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Trade> Trade { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<UserSettings> UserSettings { get; set; }
        public virtual DbSet<Cert> Certs { get; set; }
        public virtual DbSet<DailyAnalysis> DailyAnalysis { get; set; }
        public virtual DbSet<UserProducts> UserProducts { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Server=tcp:stockwolf-trading-db.database.windows.net,1433;Database=StockWolfTradingDB;Uid=stockwolfadmin;Pwd=stockWolfP!@#;Encrypt=yes;TrustServerCertificate=no;");// AppSettings.SWTConnString);
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-MI0ANF3\\SQLEXPRESS01;Initial Catalog=StockWolfTradingDB;Integrated Security=false;MultipleActiveResultSets=True;Trusted_Connection=True;MultiSubnetFailover=False;");
                //optionsBuilder.UseSqlServer("Server=DESKTOP-MI0ANF3\\SQLEXPRESS;Database=StockWolfTradingDB;Trusted_Connection=Yes");// AppSettings.SWTConnString);
                //optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SWTConnString"));
                //optionsBuilder.UseSqlServer(appSettings.SWTConnString);
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<Algorithm>(entity =>
            {
                entity.Property(e => e.AlgorithmName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateFinished).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.UserRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_User");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasOne(d => d.OrderRef)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OrderRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.ProductRef)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.ProductRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Products");
            });

            modelBuilder.Entity<OrderDetailsTicker>(entity =>
            {
                entity.Property(e => e.DateSelected).HasColumnType("datetime");

                entity.Property(e => e.TickerName)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.OrderDetailsRef)
                    .WithMany(p => p.OrderDetailsTicker)
                    .HasForeignKey(d => d.OrderDetailsRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetailsTickers_OrderDetails");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductName).IsRequired();
            });

            modelBuilder.Entity<Trade>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.Interval)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Ticker)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.AlgorithmRef)
                    .WithMany(p => p.Trade)
                    .HasForeignKey(d => d.AlgorithmRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trades_Algorithms");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.Expire).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

                entity.Property(e => e.Password).HasMaxLength(64);

                entity.Property(e => e.VerificationCode).HasMaxLength(50);
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserUserId, e.RoleRoleId })
                    .HasName("PK_dbo.UserRoles");

                entity.Property(e => e.UserUserId).HasColumnName("User_UserId");

                entity.Property(e => e.RoleRoleId).HasColumnName("Role_RoleId");

                entity.HasOne(d => d.RoleRole)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleRoleId)
                    .HasConstraintName("FK_dbo.UserRoles_Identity.Role_Role_RoleId");

                entity.HasOne(d => d.UserUser)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserUserId)
                    .HasConstraintName("FK_dbo.UserRoles_Identity.User_User_UserId");
            });

            //modelBuilder.Entity<UserProducts>(entity =>
            //{
            //    entity.HasKey(e => new { e.UserUserId, e.ProductProductId })
            //        .HasName("PK_dbo.UserProducts");

            //    entity.Property(e => e.UserUserId).HasColumnName("UserUserId");

            //    entity.Property(e => e.ProductProductId).HasColumnName("ProductProductId");

            //    entity.HasOne(d => d.ProductProduct)
            //        .WithMany(p => p.UserProducts)
            //        .HasForeignKey(d => d.ProductProductId)
            //        .HasConstraintName("FK_dbo.UserProducts_Identity.ProductProductId_ProductId");

            //    entity.HasOne(d => d.UserUser)
            //        .WithMany(p => p.UserProducts)
            //        .HasForeignKey(d => d.UserUserId)
            //        .HasConstraintName("FK_dbo.UserProducts_Identity.UserUser_UserId");
            //});

            modelBuilder.Entity<UserSettings>(entity =>
            {
                entity.Property(e => e.Interval).HasMaxLength(10);

                entity.Property(e => e.Ticker).HasMaxLength(50);

                entity.HasOne(d => d.UserRef)
                    .WithMany(p => p.UserSettings)
                    .HasForeignKey(d => d.UserRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSettings_User");
            });

            modelBuilder.Entity<Cert>(entity =>
            {
                entity.Property(f => f.Id)
                .ValueGeneratedOnAdd();
            });
        }
    }
}
