﻿// <auto-generated />
using System;
using Eva.Insurtech.FlowManagers.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Volo.Abp.EntityFrameworkCore;

namespace Eva.Insurtech.FlowManager.Migrations
{
    [DbContext(typeof(FlowManagerDbContext))]
    [Migration("20220718233154_Add-Process-Log-MaxTimes")]
    partial class AddProcessLogMaxTimes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("_Abp_DatabaseProvider", EfCoreDatabaseProvider.SqlServer)
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Eva.Insurtech.FlowManagers.Catalogs.Catalog", b =>
                {
                    b.Property<string>("Code")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<Guid>("CatalogId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Code");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Code", "CatalogId")
                        .IsUnique();

                    b.ToTable("Catalogs", "FlowManager");
                });

            modelBuilder.Entity("Eva.Insurtech.FlowManagers.Flows.Flow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ChannelCode")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifierId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MaxLifeTime")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<int>("OtpMaxAttempts")
                        .HasColumnType("int");

                    b.Property<int>("OtpMaxResends")
                        .HasColumnType("int");

                    b.Property<int>("OtpMaxTime")
                        .HasColumnType("int");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Flows", "FlowManager");
                });

            modelBuilder.Entity("Eva.Insurtech.FlowManagers.Products.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExternalCode")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Code", "ProductId", "ExternalCode")
                        .IsUnique();

                    b.ToTable("Products", "FlowManager");
                });

            modelBuilder.Entity("Eva.Insurtech.FlowManagers.Trackings.Tracking", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Abandon")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ChangeState")
                        .HasColumnType("datetime2");

                    b.Property<string>("ChannelCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("End")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<Guid>("FlowId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GeneralStateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("IpClient")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastModifierId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("StateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StepId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("WayCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Trackings", "FlowManager");
                });

            modelBuilder.Entity("Volo.Abp.AuditLogging.AuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApplicationName")
                        .HasMaxLength(96)
                        .HasColumnType("nvarchar(96)")
                        .HasColumnName("ApplicationName");

                    b.Property<string>("BrowserInfo")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)")
                        .HasColumnName("BrowserInfo");

                    b.Property<string>("ClientId")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("ClientId");

                    b.Property<string>("ClientIpAddress")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("ClientIpAddress");

                    b.Property<string>("ClientName")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("ClientName");

                    b.Property<string>("Comments")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("Comments");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("CorrelationId")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("CorrelationId");

                    b.Property<string>("Exceptions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ExecutionDuration")
                        .HasColumnType("int")
                        .HasColumnName("ExecutionDuration");

                    b.Property<DateTime>("ExecutionTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("HttpMethod")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)")
                        .HasColumnName("HttpMethod");

                    b.Property<int?>("HttpStatusCode")
                        .HasColumnType("int")
                        .HasColumnName("HttpStatusCode");

                    b.Property<Guid?>("ImpersonatorTenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ImpersonatorTenantId");

                    b.Property<Guid?>("ImpersonatorUserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ImpersonatorUserId");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TenantId");

                    b.Property<string>("TenantName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("Url");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserId");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("UserName");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "ExecutionTime");

                    b.HasIndex("TenantId", "UserId", "ExecutionTime");

                    b.ToTable("AbpAuditLogs");
                });

            modelBuilder.Entity("Volo.Abp.AuditLogging.AuditLogAction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuditLogId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("AuditLogId");

                    b.Property<int>("ExecutionDuration")
                        .HasColumnType("int")
                        .HasColumnName("ExecutionDuration");

                    b.Property<DateTime>("ExecutionTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("ExecutionTime");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("MethodName")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("MethodName");

                    b.Property<string>("Parameters")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)")
                        .HasColumnName("Parameters");

                    b.Property<string>("ServiceName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("ServiceName");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TenantId");

                    b.HasKey("Id");

                    b.HasIndex("AuditLogId");

                    b.HasIndex("TenantId", "ServiceName", "MethodName", "ExecutionTime");

                    b.ToTable("AbpAuditLogActions");
                });

            modelBuilder.Entity("Volo.Abp.AuditLogging.EntityChange", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuditLogId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("AuditLogId");

                    b.Property<DateTime>("ChangeTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("ChangeTime");

                    b.Property<byte>("ChangeType")
                        .HasColumnType("tinyint")
                        .HasColumnName("ChangeType");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("EntityId");

                    b.Property<Guid?>("EntityTenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EntityTypeFullName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("EntityTypeFullName");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TenantId");

                    b.HasKey("Id");

                    b.HasIndex("AuditLogId");

                    b.HasIndex("TenantId", "EntityTypeFullName", "EntityId");

                    b.ToTable("AbpEntityChanges");
                });

            modelBuilder.Entity("Volo.Abp.AuditLogging.EntityPropertyChange", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EntityChangeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NewValue")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)")
                        .HasColumnName("NewValue");

                    b.Property<string>("OriginalValue")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)")
                        .HasColumnName("OriginalValue");

                    b.Property<string>("PropertyName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("PropertyName");

                    b.Property<string>("PropertyTypeFullName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("PropertyTypeFullName");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TenantId");

                    b.HasKey("Id");

                    b.HasIndex("EntityChangeId");

                    b.ToTable("AbpEntityPropertyChanges");
                });

            modelBuilder.Entity("Volo.Abp.FeatureManagement.FeatureValue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("ProviderName")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("Name", "ProviderName", "ProviderKey");

                    b.ToTable("AbpFeatureValues");
                });

            modelBuilder.Entity("Volo.Abp.PermissionManagement.PermissionGrant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("ProviderName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TenantId");

                    b.HasKey("Id");

                    b.HasIndex("Name", "ProviderName", "ProviderKey");

                    b.ToTable("AbpPermissionGrants");
                });

            modelBuilder.Entity("Volo.Abp.SettingManagement.Setting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("ProviderName")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.HasKey("Id");

                    b.HasIndex("Name", "ProviderName", "ProviderKey");

                    b.ToTable("AbpSettings");
                });

            modelBuilder.Entity("Volo.Abp.TenantManagement.Tenant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreationTime");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("CreatorId");

                    b.Property<Guid?>("DeleterId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("DeleterId");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("DeletionTime");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("IsDeleted");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("LastModificationTime");

                    b.Property<Guid?>("LastModifierId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("LastModifierId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("AbpTenants");
                });

            modelBuilder.Entity("Volo.Abp.TenantManagement.TenantConnectionString", b =>
                {
                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.HasKey("TenantId", "Name");

                    b.ToTable("AbpTenantConnectionStrings");
                });

            modelBuilder.Entity("Eva.Insurtech.FlowManagers.Flows.Flow", b =>
                {
                    b.OwnsMany("Eva.Insurtech.FlowManagers.Flows.FlowStep", "FlowSteps", b1 =>
                        {
                            b1.Property<Guid>("FlowId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("StepId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("EndPointService")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<bool>("IsActive")
                                .HasColumnType("bit");

                            b1.Property<int>("MaxLifeTime")
                                .HasColumnType("int");

                            b1.Property<int>("Order")
                                .HasColumnType("int");

                            b1.Property<string>("QueueService")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("FlowId", "StepId");

                            b1.HasIndex("FlowId", "StepId")
                                .IsUnique();

                            b1.ToTable("FlowSteps", "FlowManager");

                            b1.WithOwner()
                                .HasForeignKey("FlowId");
                        });

                    b.Navigation("FlowSteps");
                });

            modelBuilder.Entity("Eva.Insurtech.FlowManagers.Trackings.Tracking", b =>
                {
                    b.OwnsMany("Eva.Insurtech.FlowManagers.Trackings.FailureLog", "FailureLogs", b1 =>
                        {
                            b1.Property<Guid>("TrackingId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTime>("RegisterTime")
                                .HasColumnType("datetime2");

                            b1.Property<string>("Detail")
                                .IsRequired()
                                .HasMaxLength(8000)
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Error")
                                .IsRequired()
                                .HasMaxLength(1024)
                                .HasColumnType("nvarchar(1024)");

                            b1.Property<string>("Method")
                                .IsRequired()
                                .HasMaxLength(64)
                                .HasColumnType("nvarchar(64)");

                            b1.Property<Guid>("StateId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("StepId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("TrackingId", "RegisterTime");

                            b1.HasIndex("TrackingId", "RegisterTime")
                                .IsUnique();

                            b1.ToTable("FailureLogs", "FlowManager");

                            b1.WithOwner()
                                .HasForeignKey("TrackingId");
                        });

                    b.OwnsMany("Eva.Insurtech.FlowManagers.Trackings.ProcessLogs.ProcessLog", "ProcessLogs", b1 =>
                        {
                            b1.Property<Guid>("TrackingId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("StepId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Version")
                                .HasColumnType("int");

                            b1.Property<string>("Action")
                                .HasMaxLength(1024)
                                .HasColumnType("nvarchar(1024)");

                            b1.Property<DateTime>("RegisterTime")
                                .HasColumnType("datetime2");

                            b1.Property<string>("Request")
                                .IsRequired()
                                .HasMaxLength(8000)
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Response")
                                .IsRequired()
                                .HasMaxLength(8000)
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("TrackingId", "StepId", "Version");

                            b1.HasIndex("TrackingId", "StepId", "Version")
                                .IsUnique();

                            b1.ToTable("ProcessLogs", "FlowManager");

                            b1.WithOwner()
                                .HasForeignKey("TrackingId");
                        });

                    b.Navigation("FailureLogs");

                    b.Navigation("ProcessLogs");
                });

            modelBuilder.Entity("Volo.Abp.AuditLogging.AuditLogAction", b =>
                {
                    b.HasOne("Volo.Abp.AuditLogging.AuditLog", null)
                        .WithMany("Actions")
                        .HasForeignKey("AuditLogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Volo.Abp.AuditLogging.EntityChange", b =>
                {
                    b.HasOne("Volo.Abp.AuditLogging.AuditLog", null)
                        .WithMany("EntityChanges")
                        .HasForeignKey("AuditLogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Volo.Abp.AuditLogging.EntityPropertyChange", b =>
                {
                    b.HasOne("Volo.Abp.AuditLogging.EntityChange", null)
                        .WithMany("PropertyChanges")
                        .HasForeignKey("EntityChangeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Volo.Abp.TenantManagement.TenantConnectionString", b =>
                {
                    b.HasOne("Volo.Abp.TenantManagement.Tenant", null)
                        .WithMany("ConnectionStrings")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Volo.Abp.AuditLogging.AuditLog", b =>
                {
                    b.Navigation("Actions");

                    b.Navigation("EntityChanges");
                });

            modelBuilder.Entity("Volo.Abp.AuditLogging.EntityChange", b =>
                {
                    b.Navigation("PropertyChanges");
                });

            modelBuilder.Entity("Volo.Abp.TenantManagement.Tenant", b =>
                {
                    b.Navigation("ConnectionStrings");
                });
#pragma warning restore 612, 618
        }
    }
}
