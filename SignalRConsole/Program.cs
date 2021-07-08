using System;
using SignalRConsole.KEyExchange;

namespace SignalRConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var alice = new User() { Name = "Alice", Id = 1 };
            alice.Keys = KeyGenerationLogic.GenerateUserKeyList(alice);
            var bob = new User() { Name = "Bob", Id = 1 };
            bob.Keys = KeyGenerationLogic.GenerateUserKeyList(bob);
            var aliceEph = KeyGenerationLogic.CreateCngKey();
            var sharedKeyA = KeyGenerationLogic.GenerateMasterKeyAsInitiator(alice.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPrivate), aliceEph, new MessengerApiClient.PublicKeyDTO() { KeyString = bob.Keys.Find(x => x.KeyType == KeyType.SignedKeyPublic).KeyString }, new MessengerApiClient.PublicKeyDTO() { KeyString = bob.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPublic).KeyString });
            var sharedKeyB = KeyGenerationLogic.GenerateMasterKeyAsReactor(bob.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPrivate), bob.Keys.Find(x => x.KeyType == KeyType.SignedKeyPrivate), new MessengerApiClient.PublicKeyDTO() { KeyString = alice.Keys.Find(x => x.KeyType == KeyType.SignedKeyPublic).KeyString }, new MessengerApiClient.PublicKeyDTO() { KeyString = alice.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPublic).KeyString });
            Console.WriteLine(sharedKeyA.KeyString.ToString());
            Console.WriteLine(sharedKeyB.KeyString.ToString());
            Console.ReadKey();
        }
    }
}
/*
             Console.WriteLine("Hello World!");
            SignalRClient signalRClient = new SignalRClient();
            Console.WriteLine("Connecting...");
            signalRClient.StartAsync().GetAwaiter().GetResult();
            if (signalRClient.IsConnected)
            {
                Console.WriteLine("Connected.");
            }
            while (signalRClient.IsConnected)
            {

            }
 
 */