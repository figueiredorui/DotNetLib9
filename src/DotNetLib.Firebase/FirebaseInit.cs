using FirebaseAdmin;
using Google.Api.Gax.ResourceNames;
using Google.Apis.Auth.OAuth2;
using System.Reflection;

namespace DotNetLib.Firebase
{
    public class FirebaseInit
    {
        public static void FromFile(string path)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(path)
            });
        }
    }
}