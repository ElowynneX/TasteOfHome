namespace TasteOfHome.Data
{
    public static class FakeUsers
    {
        public static List<(string Email, string Password)> Users =
            new()
            {
                ("test@toh.com", "password"),
                ("admin@toh.com", "admin")
            };
        public static string? LoggedInEmail { get; set; }
    }
}
