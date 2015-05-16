using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace Merkz_Server
{
    //External Functions/Interfaces
    //--------------------------------------
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
    //======================================

    public class Server_TCP
    {
//Server Specific Variables    
//--------------------------------------
    const int BUFFER_MAX = 1024;
    const int PORT_NUM = 11000;

    // System.Threading.ManualResetEvent
    public static ManualResetEvent allDone = new ManualResetEvent(false);
//======================================



        public Server_TCP()
        { 
        
        }

        //Here we go into an endless Loop untill the Server is
        //forced to Quit
        public void Start_Listening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[BUFFER_MAX];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".

            //Dns.GetHostName()
            IPHostEntry ipHostInfo = Dns.Resolve("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            //Link ipAddress to a Port in the .Net Abstract Class endpoint
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT_NUM);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    //Async Callback : Creates a seperate thread that performs a Function when X
                    //in this case X = Accept
                    listener.BeginAccept(new AsyncCallback(AcceptCallback),listener);

                    // Halt the Thread untill Signalled to Continue
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        //Server CallBacks:
        //--------------------------------------

        //Accept
        public static void AcceptCallback(IAsyncResult asyncResult)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)asyncResult.AsyncState;
            Socket handler = listener.EndAccept(asyncResult);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        //ReadCallBack, Called From Client when a Message is Recieved
        public static void ReadCallback(IAsyncResult asyncResult)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)asyncResult.AsyncState;
            Socket clientSocket = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = clientSocket.EndReceive(asyncResult);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client.
                   // Send(clientSocket, content);


                    string message = "[From Server:] I have Recieved your message Jedi...";
                    Send(clientSocket, message);
                }
                else
                {
                    // Not all data received. Get more.
                    clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }
        //Send Callback
        private static void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)asyncResult.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(asyncResult);
                Console.WriteLine("[From Server] Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        //======================================

        private static void Send(Socket clientSocket, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            //Call the Async Callback to send the Message to the Client
            clientSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), clientSocket);
        }



    }
}
