using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace UploadServer;

internal class Program
{
    private static readonly int __Received = 0x114514;

    static void Main(string[] args)
    {
        IPEndPoint iPEndPoint = new(IPAddress.Parse(args[0]), int.Parse(args[1]));

        Socket Server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Server.Bind(iPEndPoint);

        Server.Listen(10);

        Thread thr = new(Accept);
        thr.IsBackground = true;
        thr.Start(Server);
    }

    private static void Accept(object socket)
    {
        Socket server = socket as Socket;

        Socket acc = server.Accept();

        byte[] filelistdata = File.ReadAllBytes(Path.GetTempPath() + "DownUpFilesWindows.FilesList.json");

        acc.Send(filelistdata);
        byte[] flag = new byte[4];
        acc.Receive(flag);
        if (flag[..3].Equals(__Received))
        {
            Thread thread = new Thread(Receive);
            thread.IsBackground = true;
            thread.Start(acc);
        }

        Accept(server);
    }

    private static void Receive(object socket)
    {
        Socket user = socket as Socket;
    }

    private static void Close()
    {
        throw new NotImplementedException();
    }
}