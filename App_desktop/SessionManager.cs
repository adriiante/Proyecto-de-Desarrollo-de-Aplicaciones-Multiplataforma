namespace InfoRace
{
    class SessionManager
    {
        public static UserData? CurrentUser { get; private set; }
        public static bool IsLoggedIn => CurrentUser != null;

        public static void Login(UserData user)
        {
            CurrentUser = user;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
