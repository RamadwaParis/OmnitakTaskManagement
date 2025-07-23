namespace OmintakProduction.Data
{
    public static class ManualRoleUpdater
    {
        public static void PrintUpdateScript()
        {
            Console.WriteLine("==== Manual Role Update Script ====");
            Console.WriteLine();
            Console.WriteLine("To manually update user roles in your database, run these SQL commands:");
            Console.WriteLine();
            
            Console.WriteLine("-- First, ensure the TeamLead role exists");
            Console.WriteLine("IF NOT EXISTS (SELECT 1 FROM [Role] WHERE RoleName = 'TeamLead')");
            Console.WriteLine("BEGIN");
            Console.WriteLine("    INSERT INTO [Role] (RoleName) VALUES ('TeamLead')");
            Console.WriteLine("END");
            Console.WriteLine();
            
            Console.WriteLine("-- Update Dumisani Nxumalo to Team Lead");
            Console.WriteLine("UPDATE [User] SET RoleId = (SELECT RoleId FROM [Role] WHERE RoleName = 'TeamLead')");
            Console.WriteLine("WHERE Email = 'dumisaninxumalo5gmail.com' OR (FirstName = 'Dumisani' AND LastName = 'Nxumalo')");
            Console.WriteLine();
            
            Console.WriteLine("-- Update Thabang Ledwaba to Stakeholder");
            Console.WriteLine("UPDATE [User] SET RoleId = (SELECT RoleId FROM [Role] WHERE RoleName = 'Stakeholder')");
            Console.WriteLine("WHERE Email = 'tledwaba@dynamicdna.co.za' OR (FirstName = 'Thabang' AND LastName = 'Ledwaba')");
            Console.WriteLine();
            
            Console.WriteLine("-- Update Paris Ramadwa to Tester");
            Console.WriteLine("UPDATE [User] SET RoleId = (SELECT RoleId FROM [Role] WHERE RoleName = 'Tester')");
            Console.WriteLine("WHERE Email = 'ramadwaparis@gmail.com' OR (FirstName = 'Paris' AND LastName = 'Ramadwa')");
            Console.WriteLine();
            
            Console.WriteLine("-- Update Zilungile Nquku to Developer");
            Console.WriteLine("UPDATE [User] SET RoleId = (SELECT RoleId FROM [Role] WHERE RoleName = 'Developer')");
            Console.WriteLine("WHERE Email = 'ayakazilungile20@gmail.com' OR (FirstName = 'Zilungile' AND LastName = 'Nquku')");
            Console.WriteLine();
            
            Console.WriteLine("-- Verify the updates");
            Console.WriteLine("SELECT u.FirstName, u.LastName, u.Email, r.RoleName, t.TeamName");
            Console.WriteLine("FROM [User] u");
            Console.WriteLine("LEFT JOIN [Role] r ON u.RoleId = r.RoleId");
            Console.WriteLine("LEFT JOIN [Team] t ON u.TeamId = t.TeamId");
            Console.WriteLine("WHERE u.FirstName IN ('Dumisani', 'Thabang', 'Paris', 'Zilungile')");
            Console.WriteLine("ORDER BY u.FirstName");
            Console.WriteLine();
            Console.WriteLine("==== End of Manual Update Script ====");
        }
    }
}
