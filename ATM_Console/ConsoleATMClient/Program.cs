using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ConsoleATMClient
{
    class Program
    {
        private NetworkStream output;
        private BinaryWriter writer;
        private BinaryReader reader;
        private string message = "";
        private string accNumber = "";
        private string pinNumber = "";
        private string test = "";
        private string action = "";
        private string balance = "";

        static int port = 8190;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        void Run()
        {
            new Thread(Connect).Start();
        }

        void Connect()
        {
            TcpClient client = null;
            try
            {
                Console.WriteLine("Attempting connection...");
                client = new TcpClient();
                client.Connect("localhost", port);
                output = client.GetStream();
                writer = new BinaryWriter(output);
                reader = new BinaryReader(output);
                do
                {
                    try
                    {
                        message = reader.ReadString();
                        Console.WriteLine(message);
                        if (message == "Please type in your Account number or type CANCEL")
                        {
                            accNumber = Console.ReadLine();
                            writer.Write(accNumber);
                        }
                        if (message == "Enter pin number:")
                        {
                            pinNumber = Console.ReadLine();
                            writer.Write(pinNumber);

                        }
                        if (message == "Any other key to exit")
                        {
                            action = Console.ReadLine();
                            writer.Write(action);
                        }

                        if (message == "Do you want to continue transactions?")
                        {
                            action = Console.ReadLine();
                            writer.Write(action);
                        }
                        if (message == "How much do you want to withdraw:")
                        {
                            balance = Console.ReadLine();
                            writer.Write(balance);
                            Console.WriteLine("Thank you for your business!");
                            Console.ReadLine();


                        }
                        if (message == "How much do you want to add to your account? 50000$ max")
                        {
                            test = Console.ReadLine();
                            writer.Write(test);
                            Console.WriteLine("Thank you for your business!");
                            Console.ReadLine();
                        }
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.ToString());
                        Environment.Exit(Environment.ExitCode);
                    }
                } while (message != "Simulation complete. Thanks.");

            } // end try
            catch (Exception error)
            {
                // handle exception if error in establishing connection
                Console.WriteLine(error.ToString());
                Environment.Exit(Environment.ExitCode);
            } // end catch
            finally
            {
                reader.Close();
                writer.Close();
                output.Close();
                client.Close();
            }
            Console.ReadLine();
        }
    }
}
