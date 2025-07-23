using System;
using BCrypt.Net;

// Quick hash generator
Console.WriteLine("Admin@123: " + BCrypt.HashPassword("Admin@123"));
Console.WriteLine("User@123: " + BCrypt.HashPassword("User@123"));
