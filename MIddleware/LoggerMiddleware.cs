using log4net;
using System.Text;

public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LoggerMiddleware));
        public LoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext context)
        {
            string requestId = Guid.NewGuid().ToString();
            var request = await FormatRequest(context.Request);
            var isLog = context.Request.Path != "/";
            if (isLog)
                _logger.Info($"Request {requestId} | URL={request} | Token={context.Request.Headers["Authorization"]}");
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                var response = await FormatResponse(context.Response);

                if (isLog)
                    _logger.Info($"Response {requestId} | URL={request} | Response={response}");
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {

            var body = request.Body;
            request.EnableBuffering();
            request.Body.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            request.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            //request.Body = body;

            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"{response.StatusCode}: {text}";
        }
    }