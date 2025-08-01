using System;
using BCrypt.Net;

class HashGenerator
{
    static void Main()
    {
        // Generate proper BCrypt hashes
        string adminHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        string userHash = BCrypt.Net.BCrypt.HashPassword("User@123");
        
        Console.WriteLine($"Admin@123 hash: {adminHash}");
        Console.WriteLine($"User@123 hash: {userHash}");
        
        // Verify the hashes work
        Console.WriteLine($"Admin verification: {BCrypt.Net.BCrypt.Verify("Admin@123", adminHash)}");
        Console.WriteLine($"User verification: {BCrypt.Net.BCrypt.Verify("User@123", userHash)}");
    }
}
