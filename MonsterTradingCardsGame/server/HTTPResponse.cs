namespace MonsterTradingCardsGame.server
{
    public class HTTPResponse
    {

        public int ResponseCode { get; set; } = 200;
        public string ResponseMessage { get; set; } = "OK";

        public string? Content { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new();

        private StreamWriter writer;

        public HTTPResponse(StreamWriter writer)
        {
            this.writer = writer;
        }

        public void SendResponse()
        {
            // ----- 3. Write the HTTP-Response -----
          //  var writerAlsoToConsole = new StreamTracer(writer);  // we use a simple helper-class StreamTracer to write the HTTP-Response to the client and to the console

            writer.WriteLine($"HTTP/1.0 {ResponseCode} {ResponseMessage}");    // first line in HTTP-Response contains the HTTP-Version and the status code
            foreach (var header in Headers)
            {
                writer.WriteLine($"{header.Key}:{header.Value}");
            }
                // the HTTP-headers (in HTTP after the first line, until the empy line)
            writer.WriteLine();
            if (Content != null) { writer.WriteLine(Content); }   // the HTTP-content (here we just return a minimalistic HTML Hello-World)

            Console.WriteLine("========================================");
        }
    }
}