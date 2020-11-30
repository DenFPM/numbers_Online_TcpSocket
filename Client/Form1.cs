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

        static public List<Button> buttonList = new List<Button>();
        static private int[] winCombination = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
        static Player player = new Player(PlayerName, 1, null);
        static List<Player> players = new List<Player>();
        class Player
        {
            public List<Button> clickCombinationUser = new List<Button>();
            public string Name { get; set; }
            public int ID { get; set; }
            public string ButtonText { get; set; }
            public Player(string name,int id, string buttonText)
            {
                Name = name;
                ID = id;
                ButtonText = buttonText;
            }
            

        }
        
        static public string PlayerName { get; set; }
        static public int IdPlayer { get; set; }
        enum PacketInfo
        {
            ID, ButtonUser
        }
        static public string CurrentButtonText { get; set; }
        
        public Form1()
        {
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
            GenerationButtonsPosition();
            ShowMessageServer("get id");
            SendPacket(PacketInfo.ID);
            IdPlayer = ReceivePacket();
            ShowMessageServer("current id = " + IdPlayer);
            Thread.Sleep(1000);
            player = new Player(PlayerName, IdPlayer, null);


        }
        public void ShowMessageServer(string text = "Lets do autorization")
        {
            MessageBox.Show(text);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            PlayerName = "Default";
           PlayerName =  textBox1.Text;
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
                buttonList[i].Text = winCombinationClone[i];
            }
        }
        void MyButtonClick(object sender, EventArgs e)
        {
            Button but = sender as Button;
            player.clickCombinationUser.Add(but);
            but.Enabled = false;
            CurrentButtonText = but.Text;


            int counter = 0;

            if (player.clickCombinationUser.Count >= 1)
            {


                for (int i = 0; i < player.clickCombinationUser.Count; i++)
                {
                    counter = i;
                    button26.Enabled = false;
                    button26.BackColor = Color.Red;
                    button26.Text = Convert.ToString(counter + 1);

                    if (player.clickCombinationUser[i].Text!=Convert.ToString(winCombination[i]))
                    {
                        MessageBox.Show($"Error, try again. Your score = {counter + 1}");
                        for (int j = 0; j < player.clickCombinationUser.Count; j++)
                        {
                            player.clickCombinationUser[j].Enabled = true;
                        }
                        player.clickCombinationUser.Clear();
                        GenerationButtonsPosition();
                        break;
                    }

                }



            }
           


        }
        static void SendPacket(PacketInfo info)
        {
            switch (info)
            {
                case PacketInfo.ID:
                    writer.Write(0);
                    socket.Send(ms.GetBuffer());
                    break;
                case PacketInfo.ButtonUser:
                    player = new Player(PlayerName, IdPlayer, CurrentButtonText);
                    writer.Write(1);
                    writer.Write(player.ID);
                    writer.Write(player.ButtonText);
                    break;
            }
        }
        static int ReceivePacket()
        {
            socket.Receive(ms.GetBuffer());
            int codeRequest = reader.ReadInt32();

            int id;
            string name;
            string buttonText;
            switch (codeRequest)
            {
                case 0:
                    return reader.ReadInt32();
                case 1:
                    id = reader.ReadInt32();
                    name = reader.ReadString();
                    buttonText = reader.ReadString();

                    Player plr = players.Find(p => p.ID == id);
                    if(plr != null)
                    {
                        plr.Name = name;
                        plr.
                    }
                    break;
            }
            return -1;
        }
    }
}
