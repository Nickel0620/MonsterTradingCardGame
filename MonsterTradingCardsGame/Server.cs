using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
//http://localhost:10001/ url for browser
Console.WriteLine("The first simple http: http://localhost:10001/");

var httpServer = new TcpListener(IPAddress.Loopback, 10001);
httpServer.Start();


while (true)
{
    var clientSocket = httpServer.AcceptTcpClient();
    using var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
    using var reader = new StreamReader(clientSocket.GetStream());

    // read the request
    string? line;
    int contentLength = 0;
    //Read.Line ist blockierend, wartet bis zeile geschickt und wartet bis das geschieht
    while ((line = reader.ReadLine()) != null)
    {
        Console.WriteLine(line);
        if (line == "")
        {
            break; //end the request without content
        }
        if (line.StartsWith("Content-Length:"))
        {
            contentLength = int.Parse(line.Substring(16).Trim());
        }
    }
    //read existing content
    if( contentLength > 0)
    {
        StringBuilder content = new();
        int totalBytesRead = 0;
        var buffer = new char[1024];

        while (totalBytesRead < contentLength) {

            
            
            var bytesRead = reader.Read(buffer, 0, contentLength);
            content.Append(buffer, 0, bytesRead);
        }
    }


    //write the HTTP-response
    writer.WriteLine("HTTP/1.0 200 OK");

    writer.WriteLine("ContentType: text/html; charset=utf-8");
    writer.WriteLine();
    writer.WriteLine("<html><body><h1>Hello World!</h1></body></html>");

}

//server und client in eigene klassen aufteilen
//request parser auch eigene klasse???
//HTTP server  POST http://localhost:10001/users //CURL von moodle

//.yaml --> swagger editor zum lesen

//Server ist single threaded, multithread einbauen


//REST verwendet Token zur authentification