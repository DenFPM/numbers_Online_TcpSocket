using System;
using System.Collections.Generic;
using LiteDB;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;

namespace Numbers_Online_v2._0
{
    class Server
    {

        public class User
        {
            
            public string Name { get; set; }
            public int Score { get; set; }

            public override string ToString()
            {
                return string.Format("Id : {0}, Score : {1}",
                    Name,
                    Score
                    );
            }
        }
       

        static private int[] winCombination = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
        class Client
        {
            public Socket Socket { get; set; }
            public int ID { get; set; }
            public Client(Socket socket,int id=10001)
            {
                Socket = socket;
                ID=id;
            }
        }
        static Random random = new Random();
        static List<Client> clients = new List<Client>();
        
        static public int EnemyCounter { get; set; }
        
        static public int CurrentButton { get; set; }
        static public string CurrentName { get; set; }
        static public bool Win { get; set; }
        static public bool Erorr { get; set; }

       static  LiteDatabase db = new LiteDatabase(@"MyLiteData.db");
        static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            
            Console.Title = "Server";
            socket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            socket.Listen(10);
            
            socket.BeginAccept(AcceptCallback, null);
            Console.ReadLine();
        }

        static void AcceptCallback(IAsyncResult ar)
        {
            byte[] clientInfo = new byte[256];

            Client client = new Client(socket.EndAccept(ar),GenerationID());
            Thread thread = new Thread(HandleClient);
            thread.Start(client);
            Console.WriteLine($"Новое подключение clientID = {client.ID} " );

            
            string forotherUserU = Convert.ToString(client.ID) + " " + Convert.ToString(Win) + " " + Convert.ToString(Erorr) + " " + "true";
            clientInfo = Encoding.ASCII.GetBytes(forotherUserU);
            client.Socket.Send(clientInfo);
         
            clients.Add(client);
            socket.BeginAccept(AcceptCallback, null);
        }
        static void HandleClient(object o)
        {
            Client client = (Client)o;
            
            byte[] clientInfo = new byte[256];
            string []info;
            string setFromUser;
            
                while (true)
                {
                    List<User> getAllUser = GetAll(db);

                    try
                    {


                        client.Socket.Receive(clientInfo);
                        setFromUser = System.Text.Encoding.Default.GetString(clientInfo);
                        info = setFromUser.Split(new char[] { ' ' });
                        CurrentButton = Convert.ToInt32(info[0]);
                        EnemyCounter = Convert.ToInt32(info[1]);
                        CurrentName = info[2];
                        Console.WriteLine($"get from {client.ID}: buttonUser = {CurrentButton} | counterMove = {EnemyCounter}| Name = {CurrentName}");
                        //creats user in DB
                        
                        if (getAllUser.Count == 0)
                        {
                            CreateUserBD(CurrentName, db);
                        }

                        for (int i = 0; i < getAllUser.Count; i++)
                        {
                            if (getAllUser[i].Name == CurrentName)
                            {
                                continue;
                            }
                            if (getAllUser[i].Name != CurrentName)
                            {
                                CreateUserBD(CurrentName, db);
                            }
                        }
                        //set data in DB
                        for (int i = 0; i < getAllUser.Count; i++)
                        {
                            if (getAllUser[i].Name == CurrentName)
                            {
                                SetUserToDB(CurrentName, EnemyCounter, db);
                            }
                        }


                        CheckCurrentButtom(info[0], info[1]);
                        //EnemyCounter = Convert.ToInt32(info[0]);

                    }
                    catch
                    {
                        client.Socket.Shutdown(SocketShutdown.Both);
                        client.Socket.Disconnect(true);
                        clients.Remove(client);
                        Console.WriteLine($"Пользователь под номером {client.ID} отключилася");
                        return;
                    }


                    string forotherUser = Convert.ToString(client.ID) + " " + Convert.ToString(Win) + " " + Convert.ToString(Erorr) + " " + EnemyCounter;
                    Console.WriteLine($"id = {client.ID} | move = {CurrentButton} | counter = {EnemyCounter} кек| winner? {Win} | Losser? {Erorr}");

                    byte[] byteArrForOtherUser = new byte[256];
                    byteArrForOtherUser = Encoding.ASCII.GetBytes(forotherUser);



                    string forotherUserMe = Convert.ToString(client.ID) + " " + Convert.ToString(Win) + " " + Convert.ToString(Erorr);


                    byte[] byteArrForOtherUserMe = new byte[256];
                    byteArrForOtherUserMe = Encoding.ASCII.GetBytes(forotherUserMe);
                    
                    
                    foreach (var c in clients)
                    {
                        if (c != client)
                        {
                            c.Socket.Send(byteArrForOtherUser);
                        }
                        else
                        {
                            if (c == client)
                            {
                                c.Socket.Send(byteArrForOtherUserMe);
                            }
                        }

                    }


                }
            

        }
        
        
        static public void CheckCurrentButtom(string button,string counter)
        {
            int buttonInt = Convert.ToInt32(button);
            int counterInt = Convert.ToInt32(counter);
            for(int i = counterInt-1; i < counterInt; i++)
            {
                if(buttonInt == winCombination[i])
                {
                    if (i+1 == 3)
                    {
                        Win = true;
                        
                        
                    }
                    break;
                }
                
                if(buttonInt != winCombination[i])
                {
                    Erorr = true;
                }
            }
            
        }
        static public int GenerationID()
        {
            int id;
            while (true)
            {
                id = random.Next(0, 1001);
                if (clients.Find(c => c.ID == id) == null)
                {
                    return id;
                }
            }
        }
        static private List<User> GetAll(LiteDatabase db)
        {
            var list = new List<User>();
            
                var col = db.GetCollection<User>("user");
                foreach (User _id in col.FindAll())
                {
                    list.Add(_id);
                }
           
            return list;
        }
        static public void CreateUserBD(string name, LiteDatabase db)
        {
           
                var col = db.GetCollection<User>("user");

                var user = new User
                {
                    Name = name,
                    Score = 0

                };
                col.Insert(user);


            
        }
        static public void SetUserToDB(string name , int score, LiteDatabase db)
        {
            
                
              
                    var col = db.GetCollection<User>("user");

                    var user = new User
                    {
                        Name = name,
                        Score = score+1

                    };
                    col.Insert(user);


                
            
        }
    }
}
