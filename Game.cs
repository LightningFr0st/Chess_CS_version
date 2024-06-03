using DisplayObjects;
using MenuObjects;
using Chess;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static Utilities.TemplateGenerator;
using System.DirectoryServices;
using static System.Net.Mime.MediaTypeNames;

namespace oop2
{
    internal class Game
    {
        // NETWORK DATA

        IPEndPoint endPoint;

        TcpClient? client;

        TcpListener? host;

        NetworkStream? stream;

        public Chess game_form;

        //Game data
        public static int fieldX1 = 0, fieldY1 = 0;
        public static int fieldX2 = 800, fieldY2 = 800;
        public static int borderThickness = 25;

        private char playerColor = 'w';

        private ChessBoard? chess_board;
        private GameField start_field;
        private GameField end_field;
        //Menu main_menu;

        public Game()
        {
            start_field = ObjectGenerator.GenStartField(0, 0, 400, 400, 0);
            start_field.main_menu = GetMenu();
        }

        public async void StartGame()
        {
            if (SERVER)
            {
                if (host == null || client == null || stream == null)
                {
                    host = new TcpListener(endPoint);
                    host.Start();
                    client = await host.AcceptTcpClientAsync();
                    stream = client.GetStream();
                }
                byte[] send = new byte[1];
                if (playerColor == 'w')
                {
                    send[0] = (byte)'b';
                }
                else if (playerColor == 'b')
                {
                    send[0] = (byte)'w';
                }
                else
                {
                    Random rand = new Random();
                    double p = rand.NextDouble();
                    if (p > 0.5)
                    {
                        playerColor = 'b';
                        send[0] = (byte)'w';
                    }
                    else
                    {
                        playerColor = 'w';
                        send[0] = (byte)'b';
                    }
                }
                await stream.WriteAsync(send);
                game_started = true;
                game_ended = false;
            }
            else
            {
                if (client == null || stream == null)
                {
                    client = new TcpClient();
                    await client.ConnectAsync(endPoint);
                    stream = client.GetStream();
                }

                byte[] recieve = new byte[1];
                Thread.Sleep(500);
                await stream.ReadAsync(recieve);
                playerColor = (char)recieve[0];

                Console.WriteLine(playerColor);

                game_started = true;
                game_ended = false;
            }

            if (playerColor == 'b')
            {
                WAITING = true;
            }
            else
            {
                WAITING = false;
            }
            if (game_started)
            {
                fieldX2 += borderThickness * 2;
                fieldY2 += borderThickness * 2;
                ObjectGenerator.setPlayersColors(playerColor);
                chess_board = ObjectGenerator.GenChessBoard(fieldX1, fieldY1, fieldX2, fieldY2, borderThickness);
                game_form.RenewSize(GetSize());
            }
        }

        private bool WAITING;
        private bool SERVER = true;
        public bool game_started = false;
        public bool game_ended = false;
        public bool HandleClick(int x, int y)
        {
            if (game_started)
            {
                if (WAITING)
                {
                    GetMessage();
                    if (chess_board.game_over)
                    {
                        SendMessage('W');
                        game_ended = true;
                        game_started = false;
                        end_field = ObjectGenerator.GenEndField(200, 200, 600, 600, 3, false);
                        end_field.AddObject(GetEndMenu());
                        GetMessage();
                    }
                }
                else
                {
                    chess_board.Click(x, y);
                    if (chess_board.ENDTURN)
                    {
                        SendMessage('M');
                        WAITING = true;
                    }
                }
            }
            else if (game_ended)
            {
                end_field.Click(x, y);
            }
            else
            {
                start_field.Click(x, y);
            }
            return game_started;
        }

        public void DrawGame(Graphics g)
        {
            if (game_started)
            {
                chess_board.Draw(g);
            }
            else if (game_ended)
            {
                chess_board.Draw(g);
                end_field.Draw(g);
            }
            else
            {
                start_field.Draw(g);
            }
        }


        //NETWORK COMMUNICATION


        private async void GetMessage()
        {
            byte[] recieve = new byte[5];
            try
            {
                await stream.ReadAsync(recieve);
                if ((char)recieve[0] == 'M')
                {
                    chess_board.EnemyMove(recieve[1], recieve[2], recieve[3], recieve[4]);
                }
                else if ((char)recieve[0] == 'W')
                {
                    game_ended = true;
                    game_started = false;
                    end_field = ObjectGenerator.GenEndField(200, 200, 600, 600, 3, true);
                    end_field.AddObject(GetEndMenu());
                    GetMessage();
                }
                else if ((char)recieve[0] == 'R')
                {
                    TextObject result = new TextObject("Rematch?", 100, 150, 300, 220, null, 20, null);
                    end_field.AddObject(result);
                    end_field.main_menu = GetEndMenu_2();
                }
                else if ((char)recieve[0] == 'Y')
                {
                    StartGame();
                }
                else if ((char)recieve[0] == 'Q')
                {
                    BackToMenu();
                }
                else if ((char)recieve[0] == 'w' || (char)recieve[0] == 'b')
                {
                    playerColor = (char)recieve[0];
                }

                WAITING = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Stream closed");
            }
        }

        private async void SendMessage(char type)
        {
            byte[] send = [];
            if (type == 'M')
            {
                send = [(byte)type, (byte)chess_board.STARTI, (byte)chess_board.STARTJ, (byte)chess_board.DESTI, (byte)chess_board.DESTJ];
            }
            else
            {
                send = [(byte)type];
            }

            await stream.WriteAsync(send);
        }

        public (int, int) GetClientOffset()
        {
            return (chess_board.clientX1, chess_board.clientY1);
        }
        public (int, int) GetSize()
        {
            return chess_board.GetSize;
        }

        //MENU GENERATION

        private Menu GetMenu()
        {
            TemplateGenerator template;
            template = new TemplateGenerator(Primitive.P_Rectangle, 150, 80, Color.FromArgb(173, 95, 0), null, 2, null, 14);
            template.offsetX = 20;

            MenuItem HostBtn = template.GetTemplatePrimitive("Host");
            HostBtn.clickHandler = () =>
            {
                SERVER = true;
                start_field.main_menu = GetSecondMenu(true);
            };

            MenuItem ConnectBtn = template.GetTemplatePrimitive("Connect");
            ConnectBtn.clickHandler = () =>
            {
                SERVER = false;
                start_field.main_menu = GetSecondMenu(false);
            };


            MenuItem QuitBtn = template.GetTemplatePrimitive("Quit");
            QuitBtn.clickHandler = () =>
            {
                System.Windows.Forms.Application.Exit();
            };


            Menu menu = new Menu(10, 50);

            int offsx, offsy;
            template.direction = SubMenuDirection.Dir_Vertical;
            (offsx, offsy) = template.GenerateOffsets(0, 10, SubMenuDirection.Dir_Vertical);
            menu.offsetX = offsx;
            menu.offsetY = offsy;
            menu.AddItem(HostBtn);
            menu.AddItem(ConnectBtn);
            menu.AddItem(QuitBtn);
            return menu;
        }

        private Menu GetSecondMenu(bool isHost)
        {
            TemplateGenerator template;
            template = new TemplateGenerator(Primitive.P_Rectangle, 150, 50, Color.FromArgb(173, 95, 0), null, 2, null, 10);
            template.offsetX = 20;

            int? port = null;

            IPAddress? host_addr = null;

            MenuItem HostAdress = template.GetTemplatePrimitive($"IP:\n");
            HostAdress.clickHandler = () =>
            {
                string result = Interaction.InputBox("Enter IP", "Host Setup", "127.0.0.1");
                try
                {
                    host_addr = IPAddress.Parse(result);
                    HostAdress.text.textData = "IP:\n" + host_addr.ToString();
                }
                catch (FormatException e)
                {
                    MessageBox.Show("Invalid IP address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            MenuItem HostPort = template.GetTemplatePrimitive($"PORT:\n");
            HostPort.clickHandler = () =>
            {
                string result = Interaction.InputBox("Enter port", "Host Setup", "25");
                port = Int32.Parse(result);
                HostPort.text.textData = "PORT:\n" + port.ToString();
            };

            MenuItem ConnectBtn;
            MenuItem? PlayerColor = null;
            if (isHost)
            {
                PlayerColor = template.GetTemplatePrimitive($"Color : white");
                PlayerColor.clickHandler = () =>
                {
                    string str = "wbr";
                    int ind = str.IndexOf(playerColor);
                    playerColor = str[(ind + 1) % 3];
                    switch (playerColor)
                    {
                        case 'b':
                            PlayerColor.text.textData = $"Color : black";
                            break;
                        case 'w':
                            PlayerColor.text.textData = $"Color : white";
                            break;
                        case 'r':
                            PlayerColor.text.textData = $"Color : random";
                            break;
                    }
                };

                ConnectBtn = template.GetTemplatePrimitive("Host");
            }
            else
            {
                ConnectBtn = template.GetTemplatePrimitive("Connect");
            }

            ConnectBtn.clickHandler = () =>
            {
                if (port != null && host_addr != null)
                {
                    endPoint = new IPEndPoint(host_addr, (int)port);
                    ConnectBtn.text.textData = "Waiting for connection";
                    StartGame();
                }
            };

            MenuItem BackBtn = template.GetTemplatePrimitive("Back");
            BackBtn.clickHandler = () =>
            {
                start_field.main_menu = GetMenu();
            };


            Menu menu = new Menu(10, 50);

            int offsx, offsy;
            template.direction = SubMenuDirection.Dir_Vertical;
            (offsx, offsy) = template.GenerateOffsets(0, 10, SubMenuDirection.Dir_Vertical);
            menu.offsetX = offsx;
            menu.offsetY = offsy;
            menu.AddItem(HostAdress);
            menu.AddItem(HostPort);

            if (PlayerColor != null)
            {
                menu.AddItem(PlayerColor);
            }

            menu.AddItem(ConnectBtn);
            menu.AddItem(BackBtn);

            return menu;
        }

        private Menu GetEndMenu()
        {
            TemplateGenerator template;
            template = new TemplateGenerator(Primitive.P_Rectangle, 155, 50, Color.FromArgb(173, 95, 0), null, 2, null, 10);
            template.direction = SubMenuDirection.Dir_Horizontal;
            template.offsetX = 30;

            int? port = null;

            byte[] send = [];

            IPAddress? host_addr = null;

            MenuItem RematchBtn = template.GetTemplatePrimitive($"Rematch\n");
            RematchBtn.clickHandler = () =>
            {
                SendMessage('R');

                RematchBtn.text.textData = "Waiting for response";
            };

            MenuItem BackBtn = template.GetTemplatePrimitive("Quit");
            BackBtn.clickHandler = () =>
            {
                SendMessage('Q');
                BackToMenu();
            };


            Menu menu = new Menu(30, 300);

            int offsx, offsy;

            (offsx, offsy) = template.GenerateOffsets(30, 0, SubMenuDirection.Dir_Horizontal);
            menu.offsetX = offsx;
            menu.offsetY = offsy;
            menu.AddItem(RematchBtn);
            menu.AddItem(BackBtn);

            return menu;
        }

        private void BackToMenu()
        {
            stream.Dispose();
            stream.Close();
            stream = null;

            client.Dispose();
            client.Close();
            client = null;

            if (host != null)
            {
                host.Dispose();
                host.Stop();
                host = null;
            }

            chess_board = null;
            game_ended = false;
            game_started = false;

            start_field.main_menu = GetMenu();
            game_form.RenewSize((400, 400));
        }

        private Menu GetEndMenu_2()
        {
            TemplateGenerator template;
            template = new TemplateGenerator(Primitive.P_Rectangle, 155, 50, Color.FromArgb(173, 95, 0), null, 2, null, 10);
            template.direction = SubMenuDirection.Dir_Horizontal;
            template.offsetX = 30;

            int? port = null;

            IPAddress? host_addr = null;

            MenuItem YesBtn = template.GetTemplatePrimitive($"Yes\n");
            YesBtn.clickHandler = () =>
            {
                SendMessage('Y');

                StartGame();
            };

            MenuItem NoBtn = template.GetTemplatePrimitive("No");
            NoBtn.clickHandler = () =>
            {
                SendMessage('Q');
                BackToMenu();
            };


            Menu menu = new Menu(30, 300);

            int offsx, offsy;

            (offsx, offsy) = template.GenerateOffsets(30, 0, SubMenuDirection.Dir_Horizontal);
            menu.offsetX = offsx;
            menu.offsetY = offsy;

            menu.AddItem(YesBtn);
            menu.AddItem(NoBtn);

            return menu;
        }

    }
}
