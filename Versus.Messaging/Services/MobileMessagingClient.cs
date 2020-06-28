using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Versus.Messaging.Interfaces;

namespace Versus.Messaging.Services
{
    public class MobileMessagingClient: IMobileMessagingClient
    {
        private readonly FirebaseMessaging _messaging;

        public MobileMessagingClient()
        {
            var app = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential
                    .FromFile("../Versus.Messaging/Credentials/serviceAccountKey.json")
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
            });           
            _messaging = FirebaseMessaging.GetMessaging(app);
        }
        
        private Message CreateNotification(string title, string body, string token)
        {    
            return new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Body = body,
                    Title = title
                },
                Data = new Dictionary<string, string>
                {
                    {"Body", body},
                    {"Title", title}
                }
            };
        }
        
        private Message CreateAndroidNotification(string title, string body, string token, Dictionary<string, string> data)
        {    
            return new Message()
            {
                Token = token,
                Android = new AndroidConfig()
                {
                    Notification = new AndroidNotification()
                    {
                        Title = title,
                        Body = body,
                        ClickAction = "GET_INVITE"
                    }
                },
                Data = data
            };
        }
        
        private Message CreateData(string title, string body, string token)
        {    
            return new Message()
            {
                Token = token,
                Data = new Dictionary<string, string>
                {
                    {"Body", body},
                    {"Title", title}
                }
            };
        }

        public async Task<object> SendNotification(string token, string title, string body)
        {
            try
            {
                var result = await _messaging.SendAsync(CreateNotification(title, body, token));
                return result;
            }
            catch (FirebaseException ex)
            {
                return ex.Message;
            }
        }

        public async Task<object> SendAndroidNotification(string token, string title, string body, 
            Dictionary<string, string> data)
        {
            try
            {
                var result = await _messaging.SendAsync(CreateAndroidNotification(title, body, token, data));
                return result;
            }
            catch (FirebaseException ex)
            {
                return ex.Message;
            }
        }

        public async Task<object> SendData(string token, string title, string body)
        {
            try
            {
                var result = await _messaging.SendAsync(CreateData(title, body, token));
                return result;
            }
            catch (FirebaseException ex)
            {
                return ex.Message;
            }
        }
    }
}