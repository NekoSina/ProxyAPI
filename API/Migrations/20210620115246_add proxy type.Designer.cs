﻿// <auto-generated />
using System;
using HerstAPI.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProxyAPI.Migrations
{
    [DbContext(typeof(HerstDbContext))]
    [Migration("20210620115246_add proxy type")]
    partial class addproxytype
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0-preview.4.21253.1");

            modelBuilder.Entity("WiFiAccessPointWiFiClient", b =>
                {
                    b.Property<ulong>("AccessPointsWiFiAccessPointId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("ClientsWiFiClientId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AccessPointsWiFiAccessPointId", "ClientsWiFiClientId");

                    b.HasIndex("ClientsWiFiClientId");

                    b.ToTable("WiFiAccessPointWiFiClient");
                });

            modelBuilder.Entity("libherst.Models.Proxy", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AS")
                        .HasColumnType("TEXT");

                    b.Property<string>("ASN")
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<string>("Domain")
                        .HasColumnType("TEXT");

                    b.Property<string>("IP")
                        .HasColumnType("TEXT");

                    b.Property<string>("ISP")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastTest")
                        .HasColumnType("TEXT");

                    b.Property<float>("Latitude")
                        .HasColumnType("REAL");

                    b.Property<float>("Longitude")
                        .HasColumnType("REAL");

                    b.Property<ushort>("Port")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ProxyType")
                        .HasColumnType("TEXT");

                    b.Property<string>("Region")
                        .HasColumnType("TEXT");

                    b.Property<int>("Score")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Threat")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Working")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Proxies");
                });

            modelBuilder.Entity("libherst.Models.UserInfo", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("libherst.Models.WiFiAccessPoint", b =>
                {
                    b.Property<ulong>("WiFiAccessPointId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastSeen")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("WiFiMacId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("WiFiNetworkNameId")
                        .HasColumnType("INTEGER");

                    b.HasKey("WiFiAccessPointId");

                    b.HasIndex("WiFiMacId");

                    b.HasIndex("WiFiNetworkNameId");

                    b.ToTable("WiFiAccessPoints");
                });

            modelBuilder.Entity("libherst.Models.WiFiClient", b =>
                {
                    b.Property<ulong>("WiFiClientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastSeen")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("WiFiMacId")
                        .HasColumnType("INTEGER");

                    b.HasKey("WiFiClientId");

                    b.HasIndex("WiFiMacId");

                    b.ToTable("WiFiClients");
                });

            modelBuilder.Entity("libherst.Models.WiFiMac", b =>
                {
                    b.Property<ulong>("WiFiMacId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MAC")
                        .HasColumnType("TEXT");

                    b.HasKey("WiFiMacId");

                    b.ToTable("WiFiMacs");
                });

            modelBuilder.Entity("libherst.Models.WiFiNetworkName", b =>
                {
                    b.Property<ulong>("WiFiNetworkNameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("SSID")
                        .HasColumnType("TEXT");

                    b.HasKey("WiFiNetworkNameId");

                    b.ToTable("WiFiNetworkNames");
                });

            modelBuilder.Entity("libherst.Models.WiFiProbe", b =>
                {
                    b.Property<ulong>("WiFiProbeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("ClientWiFiClientId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastSeen")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("WiFiMacId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("WiFiNetworkNameId")
                        .HasColumnType("INTEGER");

                    b.HasKey("WiFiProbeId");

                    b.HasIndex("ClientWiFiClientId");

                    b.HasIndex("WiFiMacId");

                    b.HasIndex("WiFiNetworkNameId");

                    b.ToTable("WiFiProbes");
                });

            modelBuilder.Entity("WiFiAccessPointWiFiClient", b =>
                {
                    b.HasOne("libherst.Models.WiFiAccessPoint", null)
                        .WithMany()
                        .HasForeignKey("AccessPointsWiFiAccessPointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("libherst.Models.WiFiClient", null)
                        .WithMany()
                        .HasForeignKey("ClientsWiFiClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("libherst.Models.WiFiAccessPoint", b =>
                {
                    b.HasOne("libherst.Models.WiFiMac", "WiFiMac")
                        .WithMany()
                        .HasForeignKey("WiFiMacId");

                    b.HasOne("libherst.Models.WiFiNetworkName", "WiFiNetworkName")
                        .WithMany()
                        .HasForeignKey("WiFiNetworkNameId");

                    b.Navigation("WiFiMac");

                    b.Navigation("WiFiNetworkName");
                });

            modelBuilder.Entity("libherst.Models.WiFiClient", b =>
                {
                    b.HasOne("libherst.Models.WiFiMac", "WiFiMac")
                        .WithMany()
                        .HasForeignKey("WiFiMacId");

                    b.Navigation("WiFiMac");
                });

            modelBuilder.Entity("libherst.Models.WiFiProbe", b =>
                {
                    b.HasOne("libherst.Models.WiFiClient", "Client")
                        .WithMany()
                        .HasForeignKey("ClientWiFiClientId");

                    b.HasOne("libherst.Models.WiFiMac", "WiFiMac")
                        .WithMany()
                        .HasForeignKey("WiFiMacId");

                    b.HasOne("libherst.Models.WiFiNetworkName", "WiFiNetworkName")
                        .WithMany()
                        .HasForeignKey("WiFiNetworkNameId");

                    b.Navigation("Client");

                    b.Navigation("WiFiMac");

                    b.Navigation("WiFiNetworkName");
                });
#pragma warning restore 612, 618
        }
    }
}