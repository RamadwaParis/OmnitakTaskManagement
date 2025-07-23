using System;

class Program
{
    static void Main()
    {
        string adminPassword = "Admin@123";
        string userPassword = "User@123";
        
        Console.WriteLine("Admin@123 hash: " + BCrypt.Net.BCrypt.HashPassword(adminPassword));
        Console.WriteLine("User@123 hash: " + BCrypt.Net.BCrypt.HashPassword(userPassword));
    }
}
