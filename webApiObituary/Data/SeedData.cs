using System;
using Assignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Data;

public static class SeedData
{
    // This is an extension method to the ModelBuilder class
    public static void Seed(this ModelBuilder modelBuilder)
    {
        var users = GetUsers();
        var roles = GetRoles();
        modelBuilder.Entity<IdentityUser>().HasData(users);
        modelBuilder.Entity<IdentityRole>().HasData(roles);
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(GetUserRoles(users, roles));

        modelBuilder.Entity<Obituary>().HasData(GetObituaries());
    }

    public static List<IdentityUser> GetUsers()
    {
        List<IdentityUser> users = new List<IdentityUser>()
        {
            new IdentityUser // Admin user
            {
                Id = "admin-user-id-1234",
                UserName = "aa@aa.aa",
                NormalizedUserName = "AA@AA.AA",
                Email = "aa@aa.aa",
                NormalizedEmail = "AA@AA.AA",
                EmailConfirmed = true,
                SecurityStamp = "7f434309-a4d9-48e9-9ebb-8803db794577",
                ConcurrencyStamp = "c8554266-b7ad-4345-b57a-6842bec5b3ab",
                PasswordHash = "AQAAAAIAAYagAAAAEDYVYGiMt0MOuCNJ1ASr6U9gFlAYiVpFnCmXQFpQCXXRTg4Zb7fg7LgAiyc2o70HZA=="
            },
            new IdentityUser // Regular user
            {
                Id = "regular-user-id-5678",
                UserName = "uu@uu.uu",
                NormalizedUserName = "UU@UU.UU",
                Email = "uu@uu.uu",
                NormalizedEmail = "UU@UU.UU",
                EmailConfirmed = true,
                SecurityStamp = "3dba6d67-4142-43e4-b4af-4b2e35f36e7f",
                ConcurrencyStamp = "d7b9a1c3-5468-4e72-8d95-8a8a7b5c2b9d",
                PasswordHash = "AQAAAAIAAYagAAAAEDYVYGiMt0MOuCNJ1ASr6U9gFlAYiVpFnCmXQFpQCXXRTg4Zb7fg7LgAiyc2o70HZA=="
            }
        };

        return users;
    }

    private static List<IdentityRole> GetRoles()
    {
        // Seed Roles
        var adminRole = new IdentityRole("Admin")
        {
            Id = "1", // Static ID instead of dynamic GUID
            ConcurrencyStamp = "1"
        };
        adminRole.NormalizedName = adminRole.Name!.ToUpper();
        var userRole = new IdentityRole("User")
        {
            Id = "2", // Static ID instead of dynamic GUID
            ConcurrencyStamp = "2"
        };
        userRole.NormalizedName = userRole.Name!.ToUpper();
        List<IdentityRole> roles = new List<IdentityRole>() {
          adminRole,
          userRole
      };
        return roles;
    }

    private static List<IdentityUserRole<string>> GetUserRoles(List<IdentityUser> users, List<IdentityRole> roles)
    {
        // Seed UserRoles
        List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();
        userRoles.Add(new IdentityUserRole<string>
        {
            UserId = "admin-user-id-1234",
            RoleId = "1"
        });
        userRoles.Add(new IdentityUserRole<string>
        {
            UserId = "regular-user-id-5678",
            RoleId = "2"
        });
        return userRoles;
    }

    public static List<Obituary> GetObituaries()
    {
        List<Obituary> obituaries = new List<Obituary>()
        {
            new Obituary
            {
                Id = 1,
                FullName = "John Doe",
                DateOfBirth = new DateOnly(1950, 5, 15),
                DateOfDeath = new DateOnly(2023, 10, 1),
                Biography = "John was a loving father and grandfather who dedicated his life to teaching. He touched the lives of countless students over his 40-year career as a high school mathematics teacher.",
                PrimaryPhotoBase64 = null,
                CreatedByUserId = "admin-user-id-1234",
                CreatedAtUtc = new DateTime(2023, 10, 2, 10, 0, 0, DateTimeKind.Utc)
            },
            new Obituary
            {
                Id = 2,
                FullName = "Mary Johnson",
                DateOfBirth = new DateOnly(1965, 8, 22),
                DateOfDeath = new DateOnly(2024, 3, 15),
                Biography = "Mary was a passionate nurse who spent 35 years caring for others at the local hospital. She was known for her kindness and dedication to her patients and colleagues.",
                PrimaryPhotoBase64 = null,
                CreatedByUserId = "regular-user-id-5678",
                CreatedAtUtc = new DateTime(2024, 3, 16, 14, 30, 0, DateTimeKind.Utc)
            },
            new Obituary
            {
                Id = 3,
                FullName = "Robert Smith",
                DateOfBirth = new DateOnly(1945, 12, 3),
                DateOfDeath = new DateOnly(2024, 1, 20),
                Biography = "Robert was a veteran who served his country with honor for 20 years. After his military service, he became a successful businessman and devoted family man.",
                PrimaryPhotoBase64 = null,
                CreatedByUserId = "admin-user-id-1234",
                CreatedAtUtc = new DateTime(2024, 1, 21, 9, 15, 0, DateTimeKind.Utc)
            },
            new Obituary
            {
                Id = 4,
                FullName = "Elizabeth Williams",
                DateOfBirth = new DateOnly(1958, 11, 8),
                DateOfDeath = new DateOnly(2024, 5, 12),
                Biography = "Elizabeth was an artist whose beautiful paintings brought joy to many. She taught art classes at the community center and mentored young artists throughout her life.",
                PrimaryPhotoBase64 = null,
                CreatedByUserId = "regular-user-id-5678",
                CreatedAtUtc = new DateTime(2024, 5, 13, 16, 45, 0, DateTimeKind.Utc)
            },
            new Obituary
            {
                Id = 5,
                FullName = "Michael Brown",
                DateOfBirth = new DateOnly(1960, 2, 14),
                DateOfDeath = new DateOnly(2024, 8, 30),
                Biography = "Michael was a dedicated firefighter who risked his life to save others for over 25 years. He was a hero to his community and a loving husband and father.",
                PrimaryPhotoBase64 = null,
                CreatedByUserId = "admin-user-id-1234",
                CreatedAtUtc = new DateTime(2024, 8, 31, 11, 20, 0, DateTimeKind.Utc)
            }
        };

        return obituaries;
    }
}
