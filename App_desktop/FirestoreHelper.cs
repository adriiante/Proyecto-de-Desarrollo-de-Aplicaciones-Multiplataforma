using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoRace
{
    internal static class FirestoreHelper
    {
        static string fireconfig = @"
        {
          ""type"": ""service_account"",
          ""project_id"": ""inforace-fde66"",
          ""private_key_id"": ""09c6350f976993c351b7418c51d63dab7496f852"",
          ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCaqKAaM18P0BNG\nFZFz79LZ74+J5aDy5BaIIUVO7KLZ8bpJK55ffstFheMkhe/WZ5rpp4Dx7qih6l7F\nV6w0ykk8kr0/gwSnusRxqJuRqNypG6o07iE3EIu97EiejbVpaz1HGos7CA7nwqME\nKjebQYeS9axxKJbAWhE0c0Ai/V8vDeHEZ2zM/tL06JoQLS4XJnv2f1L/h/f0mPLv\nTsmDCqamXgDP+4NciwIE6URerimtpOaRldR2KfheLHl6qgkP4/i0J7fVYMgCc3tv\nVQTcxMQ+1lh7wTwU4vepueewsC/FtlDbXvmhLzTlcGYlquUT/FcPXstYgCxLg22K\njDgpblSlAgMBAAECggEAC3h04hHr+65BClz7JrrOIUp6k42rvEKycRzr7l3v+MjM\nOjUOpU0wIlM7H4Ws3h1dCKCwbZZ30gUh9kekXAiiGeiq5JUBDreew5dFF9k1qc0w\nIzDY1yQBA/BhtRtpgwy9xLzxP/WUYBgNeHebxttmBL5NMISuzva0eUwQS7K4Zog8\nKhpL86DevFHVpbZMh/m4LLVo9qXNo7Z+gHOm8hxZZfsFaVpGhrb5yE4dPD4YeIgG\nhurj4bdXQgj9tt1kEu9IpHRU8KAS5W8s/KSFgCYIXxERbvVSuPeubozD9KJQRwEt\n3s6qtb1Q8xpKd8hI/NlsXDxKqzQd8FLq8srn5sgY2QKBgQDQHU81kVRE32Z6kcZd\nSSuQ3NqZsk75AU+P46d1hN1UTaIBuu4wkAtdg6PtbI6kA2135i17d/gOgn2pK9C+\n2CXhsnG7jiHUkWok1XOr5v/MfZr3KptrvnfuLWhYELYjjU2L3ODJDo7tuAD2m6Ya\nl65Oiizh1NqZRkz8RtZO1iCjjwKBgQC+PpPgZ9Qkr551wI4XyUMQrq4Ct6QkVbKt\nAucWdFSIQs1xod3kz8pMlnvbeVa3I7ZXPEY/QlpPnB4Pn0RL2dE2hAeU/89tDz8S\nfmnjwbkkkdxOY+2C5pnWa8DEvtxmh1WEbJw8WVKBFwD2J2KarH3crwONVliniZv0\nLS0vSNcaiwKBgFWXCEGGoHqmyhW0qDkrY5BzVDALxjDgWiFaRj+twfzu5KcDgvxT\n8WinQB7gG5XWlwnNj30M7ObCwf12OKaUbJfy9JVnMgCjQV0esDDNkvw92CK40iyw\nq9A3uoclcFMUxQS1dXka1Kttlm/Vua2MHjLM3wKNsM2CvRyv4WmQ5OF/AoGBAKHO\n3b2G8fPF/zPN880dXFJLECu1gAF7dj4a8uuxQ9ilYNGeR/l+FmUvtObF/FBGKqdl\n16WkTOXbNeI5p0AQYhZJE53SOd1bk1yqcmvJ8Cu/d66g5XlpozyxZ0KvxwvZXY4F\nRpk5q6eqJiI2+3Hs9hwGKzJCV6Mfsqpq5jyKapHjAoGAIUACcOqVfr6QEF7h8y01\nlBs9YVG82gaqomI64AEuNosyjWazLE9KlkBd8sDSpmiEfh2HbYpnGpuBVr+xVI8R\n4cIkpSMI3NKz8HFMLKgEhkEiCNewybkwZJFD1bxbcpRYgBlWBgBUtSsd6UWyWHGF\nYzEsTOOqwNKNuNcli3jJblM=\n-----END PRIVATE KEY-----\n"",
          ""client_email"": ""firebase-adminsdk-3s1f3@inforace-fde66.iam.gserviceaccount.com"",
          ""client_id"": ""114100159952067247158"",
          ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
          ""token_uri"": ""https://oauth2.googleapis.com/token"",
          ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
          ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-3s1f3%40inforace-fde66.iam.gserviceaccount.com"",
          ""universe_domain"": ""googleapis.com""
        } ";

        static string filepath = "";

        public static FirestoreDb? Database { get; private set; }

        public static void SetEnvironmentVariable()
        {
            filepath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())) + ".json";
            File.WriteAllText(filepath, fireconfig);
            File.SetAttributes(filepath, FileAttributes.Hidden);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            Database = FirestoreDb.Create("inforace-fde66");
            File.Delete(filepath);
        }
    }
}
