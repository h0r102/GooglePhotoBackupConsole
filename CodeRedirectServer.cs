using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole
{
	class CodeRedirectServer
	{
        public static readonly int Port = 15555;

        private HttpListener _listener = new HttpListener();
        public event Action<string> OnCodeReceived;

        public CodeRedirectServer()
		{
		}

        public void Start()
        {
            _listener.Prefixes.Add("http://localhost:" + Port.ToString() + "/");
            _listener.Start();

            WaitForRequest();
        }

        public void Close()
        {
            _listener.Stop();
            _listener.Close();
        }

        private async void WaitForRequest()
        {
            HttpListenerContext context = null;
            try
            {
                context = await _listener.GetContextAsync();
            }
            catch (Exception ex)
            {
                // エラー時の処理
            }

            if (context != null)
            {
                var response = context.Response;
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/html";

                byte[] buffer = Encoding.UTF8.GetBytes(CreateResponseHtml());
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                GetAuthorizationCode(context);

                WaitForRequest();
            }
        }

        private string CreateResponseHtml()
        {
            return $@"
                <!DOCTYPE html><html><head></head><body></body>
                  <script>
                    var hash = location.hash;
                    if(hash.length > 0) 
                    {{
                      var params = hash.split('&');     
                      if(params.length > 0 && params[0].match(/#code/)) 
                      {{
                        var code = params[0].split('=')[1];
                        window.location.replace(`http://localhost:{Port}/?code=` + code);
                      }}
                    }}
                  </script>
                </html>
            ";
        }

        private void GetAuthorizationCode(HttpListenerContext context)
        {
            var request = context.Request;

            if (!string.IsNullOrEmpty(request.QueryString["code"]))
            {
                OnCodeReceived?.Invoke(request.QueryString["code"]);
            }
        }
    }
}
