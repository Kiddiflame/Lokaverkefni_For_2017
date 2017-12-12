using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ConsoleATMServer
{
    class Program
    {
        private Socket connection; // Socket for accepting a connection
        private int counter = 0; // count the number of clients connected
        private string reikningsNr = "121199";
        private string magicPIN = "1234";
        private int balance = 300000;
        private int port = 8190;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        void Run()
        {
            new Thread(RunServer).Start();
        }

        public void RunServer()
        {
            Thread readThread; // Thread for processing incoming messages
            bool done = false;

            TcpListener listener;
            try
            {
                // Step 1: create TcpListener
                //IPAddress local = IPAddress.Parse("127.0.0.1");
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine("Waiting for connection ...");

                while (!done)
                {
                    // This is where the server sits and waits for clients
                    connection = listener.AcceptSocket();
                    counter++;
                    Console.WriteLine("Starting a new client, numbered " + counter);
                    // Start a new thread for a client
                    readThread = new Thread(GetMessages);
                    readThread.Start();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Port " + port + " may be busy. Try another.");
            }

        }

        public void GetMessages()
        {
            Socket socket = connection;
            int count = counter;
            NetworkStream socketStream = null;
            BinaryWriter writer = null;
            BinaryReader reader = null;

            try
            {
                // establish the communication streams
                socketStream = new NetworkStream(socket);
                reader = new BinaryReader(socketStream);
                writer = new BinaryWriter(socketStream);
                writer.Write("Connection successful.\n");
                string message = null;

                bool okay = false;
                for (int tries = 0; tries < 3 && !okay; tries++)
                {
                    writer.Write("Please type in your Account number or type CANCEL");
                    message = reader.ReadString();
                    Console.WriteLine("Client " + count + ":" + message);
                    okay = true;
                    switch (message)
                    {
                        case "CANCEL":
                            writer.Write("Transaction halted. Goodbye.");
                            break;
                        default:
                            if (message == reikningsNr)
                            {
                                writer.Write("Enter pin number:");
                                message = reader.ReadString();

                                if (message == magicPIN)
                                {
                                    writer.Write("1: View my balance");
                                    writer.Write("2: Withdray cash");
                                    writer.Write("3: Deposit funds");
                                    writer.Write("Any other key to exit");
                                    message = reader.ReadString();

                                    if (message == "1")
                                    {
                                        writer.Write("Your balance is " + balance + "$ on " + reikningsNr + " account");
                                        writer.Write("Do you want to continue transactions? Y/N");
                                        message = reader.ReadString();
                                        if (message == "Y")
                                        {
                                            writer.Write("1: View my balance");
                                            writer.Write("2: Withdray cash");
                                            writer.Write("3: Deposit funds");
                                            writer.Write("Any other key to exit");
                                            message = reader.ReadString();
                                        }

                                    }
                                    if (message == "2")
                                    {
                                        writer.Write("How much do you want to withdraw:");
                                        message = reader.ReadString();

                                        if (Int32.Parse(message) > 50000)
                                        {
                                            writer.Write("Max withdraw is 50000$");
                                            message = reader.ReadString();
                                            if (Int32.Parse(message) > balance)
                                            {
                                                writer.Write("You are trying to withdraw more the you have on your account");
                                            }
                                        }

                                        else
                                        {
                                            balance = balance - Int32.Parse(message);
                                            writer.Write(balance);
                                            Console.WriteLine("User: " + reikningsNr + " has withdrawn " + Int32.Parse(message) + " and has " + balance + " left ");
                                        }

                                    }
                                    if (message == "3")
                                    {
                                        writer.Write("How much do you want to add to your account? 50000$ max");
                                        if (Int32.Parse(message) > 50000)
                                        {
                                            writer.Write("50000$ max");


                                        }
                                        else
                                        {
                                            balance = balance + Int32.Parse(message);
                                            Console.WriteLine("User: " + reikningsNr + " has added " + Int32.Parse(message) + " and has " + balance + " left ");
                                        }
                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                {
                                    writer.Write("wrong pin number try again");
                                    okay = false;
                                }
                                // Transaction
                                break;
                            }
                            else
                            {
                                writer.Write("Incorrect Account number. Try again.");
                                okay = false;
                            }
                            break;
                    }
                }
                writer.Write("Simulation complete. Thanks.");

            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            finally
            {
                reader.Close();
                writer.Close();
                socketStream.Close();
                socket.Close();
            }

            }
            
           
        }
    }
