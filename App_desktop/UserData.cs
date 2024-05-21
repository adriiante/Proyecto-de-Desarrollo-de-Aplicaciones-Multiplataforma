using Google.Cloud.Firestore;

namespace InfoRace
{
    [FirestoreData]
    internal class UserData
    {
        [FirestoreProperty]
        public string Username { get; set; }
        [FirestoreProperty]
        public string Password { get; set; }
        [FirestoreProperty]
        public string Email { get; set; }
        [FirestoreProperty]
        public string Phone { get; set; }
    }
}
