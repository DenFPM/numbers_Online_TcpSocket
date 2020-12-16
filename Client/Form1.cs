using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public partial class Form1 : Form
    {
        

        static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        static MemoryStream ms = new MemoryStream(new byte[256], 0, 256, true, true);
        
        static BinaryWriter writer = new BinaryWriter(ms);
        static BinaryReader reader = new BinaryReader(ms);

        static List<Player> players = new List<Player>();
        static Player player;
        static Dictionary<string, int> infoForSend = new Dictionary<string, int>();
        static private List<Button> serviceButton = new List<Button>();
        static private List<Button> buttonList = new List<Button>();
        static private List<int> PlayersMove = new List<int>();
  
        static private int[] winCombination = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
        class Player
        {
            public int Id { get; set; }
            public string ButtonText { get; set; }
            public string Name { get; set; }
            public bool Winner { get; set; }
            public int CounterMove{get;set;}
            public Player(int id,string buttonText)
            {
                Id = id;
                ButtonText = buttonText;

            }
            public void SetEnemyScore(int score)
            {
                serviceButton[1].Text = Convert.ToString(score);
            }
        }

        static public int MyID { get; set; }
        static public int IdPlayer { get; set; }
        enum PacketInfo 
        {
            ButtonUser
        }
        
        
        public Form1()
        {

            
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            ms = new MemoryStream(new byte[256], 0, 256, true, true);
            socket.Connect("127.0.0.1", 8080);
            InitializeComponent();
            this.Height = 600;
            this.Width = 900;
            buttonList.Add(button1);
            buttonList.Add(button2);
            buttonList.Add(button3);
            buttonList.Add(button4);
            buttonList.Add(button5);
            buttonList.Add(button6);
            buttonList.Add(button7);
            buttonList.Add(button8);
            buttonList.Add(button9);
            buttonList.Add(button10);
            buttonList.Add(button11);
            buttonList.Add(button12);
            buttonList.Add(button13);
            buttonList.Add(button14);
            buttonList.Add(button15);
            buttonList.Add(button16);
            buttonList.Add(button17);
            buttonList.Add(button18);
            buttonList.Add(button19);
            buttonList.Add(button20);
            buttonList.Add(button21);
            buttonList.Add(button22);
            buttonList.Add(button23);
            buttonList.Add(button24);
            buttonList.Add(button25);
            serviceButton.Add(button26);
            serviceButton.Add(button27);
            
            GenerationButtonsPosition();
           
            Thread.Sleep(1000);
            player = new Player(IdPlayer,null);
            player.CounterMove = 0;
            Task.Run(() => { 
                while (true) 
                { 
                    ReceivePacket(); 
                } 
            });

        }
        static public void ShowMessageServer(string text = "Lets do autorization")
        {
            MessageBox.Show(text);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            /*PlayerName = "Default";
           PlayerName =  textBox1.Text;*/
        }
        static public  void GenerationButtonsPosition()
        {
            List<string> winCombinationClone = new List<string>();
            for (int i = 0; i < winCombination.Length; i++)
            {
                winCombinationClone.Add(Convert.ToString(winCombination[i]));
            }
            var rnd = new Random();
            winCombinationClone = winCombinationClone.OrderBy(item => rnd.Next()).ToList();
            for (int i = 0; i < buttonList.Count; i++)
            {
                buttonList[i].Enabled = true;
                buttonList[i].Text = winCombinationClone[i];
            }
        }
        TimeSpan totalTime;
        void MyButtonClick(object sender, EventArgs e)
        {
            Button but = sender as Button;
            
            
            
            but.Enabled = false;
            player.CounterMove++;
            player.ButtonText = but.Text;
            infoForSend.Add(player.ButtonText, player.CounterMove);
            PlayersMove.Add(Convert.ToInt32(player.ButtonText));

            for (int i=PlayersMove.Count-1;i< PlayersMove.Count; i++)
            {
                button26.Text =Convert.ToString( i + 1 );
                if (PlayersMove[i] == winCombination[i])
                {
                    
                    SendPacket(PacketInfo.ButtonUser);
                }
                else
                {
                    ShowMessageServer($"Ur score = {PlayersMove.Count}. Trye again");
                    GenerationButtonsPosition();
                    
                    for(int j = 0; j < PlayersMove.Count; j++)
                    {
                        buttonList[i].Enabled = true;
                        PlayersMove.Clear();
                    }
                }
            }
            

            //if (CurrentButtonText == "1")
            /* {
                 totalTime = new TimeSpan(0, 0, 0, 30);
                 label4.Text = $"Оставшееся время: {totalTime.ToString()}__";
                 timer1.Start();

                 timer1.Interval = 1000;

             }*/

        }
        
        static void SendPacket(PacketInfo info)
        {
            byte[] forSend = new byte[256];
            forSend = Encoding.ASCII.GetBytes(player.ButtonText + " " + Convert.ToString(player.CounterMove)+ " " + player.Name);
            socket.Send(forSend);
        }
        static void ReceivePacket()
        {
            

            byte[] infoByOtherUser = new byte[256];
            string[] info;
            socket.Receive(infoByOtherUser);
            string infoStr=Encoding.Default.GetString(infoByOtherUser); ;
            info = infoStr.Split(new char[] { ' ' });
            
            for (int i = 0; i < info.Length; i++)
            {
                if (info.Length == 3)
                {

                    
                    if (Convert.ToBoolean(info[1]))
                    {
                        ShowMessageServer($"You Winner!");
                        GenerationButtonsPosition();
                        serviceButton[0].Text = "0";
                        serviceButton[1].Text = "0";
                        break;
                    }
                    if (Convert.ToBoolean(info[2]))
                    {
                        ShowMessageServer($"Try again");
                        GenerationButtonsPosition();
                        serviceButton[0].Text = "0";
                        serviceButton[1].Text = "0";
                        break;
                    }
                    /*jsonPlayers.Add(new JsonPlayer { Id = Convert.ToInt32(info[0]), Score = MyScoreJson });
                    string outputJSON = Newtonsoft.Json.JsonConvert.SerializeObject(jsonPlayers, Newtonsoft.Json.Formatting.Indented);
                    MessageBox.Show(outputJSON);
                    File.WriteAllText(Path, outputJSON + Environment.NewLine);*/

                }

                else
                {
                    

                    serviceButton[1].Text = info[3];
                    if (Convert.ToBoolean(info[1]))
                    {
                        ShowMessageServer($"Winner {info[0]}");
                        GenerationButtonsPosition();
                        serviceButton[0].Text = "0";
                        serviceButton[1].Text = "0";
                        break;
                    }
                    if (Convert.ToBoolean(info[2]))
                    {
                        ShowMessageServer($"Losser {info[0]}");
                        GenerationButtonsPosition();
                        serviceButton[0].Text = "0";
                        serviceButton[1].Text = "0";
                        break;
                    }
                    /*jsonPlayers.Add(new JsonPlayer { Id = Convert.ToInt32(info[0]), Score = Convert.ToInt32(info[3]) });
                    string outputJSON = Newtonsoft.Json.JsonConvert.SerializeObject(jsonPlayers, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(Path, outputJSON + Environment.NewLine);*/
                }

            }
            
            



        
        }
            private void timer1_Tick(object sender, EventArgs e)
            {
            /*totalTime = totalTime.Subtract(new TimeSpan(0, 0, 0, 1));

            label1.Text = $"Оставшееся время: {totalTime.ToString()}__";

            if (totalTime.Seconds == 0)
            {
                timer1.Stop();
                MessageBox.Show("Время вышло");
                GenerationButtonsPosition();
                for (int j = 0; j < player.clickCombinationUser.Count; j++)
                {
                    player.clickCombinationUser[j].Enabled = true;
                }*/
            }

        private void button28_Click(object sender, EventArgs e)
        {
            
            
            
            player.Name = textBox1.Text;
        }
    }
}
