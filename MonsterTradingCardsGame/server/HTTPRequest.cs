using System.Text;

namespace MonsterTradingCardsGame.server
{
    public class HTTPRequest
    {
        public string Method { get; set; } = "";
        public string Path { get; set; } = "";

        public string HTTPVersion { get; set; } = "";

        public string? Content { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new();

        private StreamReader reader;

        public HTTPRequest(StreamReader reader)
        {
            this.reader = reader;
        }

        public void ParseRequest()
        {
            // ----- 1. Read the HTTP-Request -----
            string? line;

            // 1.1 first line in HTTP contains the method, path and HTTP version
            line = reader.ReadLine();
            if (line != null)
            {
                var parts = line.Split(' ');
                Method = parts[0]; 
                Path = parts[1];
                HTTPVersion = parts[2];
                Console.WriteLine(line);
            }
            // 1.2 read the HTTP-headers (in HTTP after the first line, until the empy line)
            int content_length = 0; // we need the content_length later, to be able to read the HTTP-content
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
                if (line == "")
                {
                    break;  // empty line indicates the end of the HTTP-headers
                }

                // Parse the header
                var parts = line.Split(':');
                Headers.Add(parts[0], parts[1]);
                if (parts.Length == 2 && parts[0] == "Content-Length")
                {
                    content_length = int.Parse(parts[1].Trim());
                }
            }

            // 1.3 read the body if existing
            if (content_length > 0)
            {
                var data = new StringBuilder(200);
                char[] chars = new char[1024];
                int bytesReadTotal = 0;
                while (bytesReadTotal < content_length)
                {
                    var bytesRead = reader.Read(chars, 0, chars.Length);
                    bytesReadTotal += bytesRead;
                    if (bytesRead == 0)
                        break;
                    data.Append(chars, 0, bytesRead);
                }
                Console.WriteLine(data.ToString());
                Content = data.ToString();
            }
        }
    }
}