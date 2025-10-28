using System;
using Microsoft.EntityFrameworkCore;
using APIApp.Models;

namespace APIApp.Data;

public class ApplicationDbContext: DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
	}
    public DbSet<User> Users { get; set; }
}
