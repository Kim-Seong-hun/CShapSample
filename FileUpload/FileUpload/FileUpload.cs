using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Encryption;


namespace FileUpload
{
    class FileUpload
    {
        private Dictionary<string, object> _memonyPostParameters;
        private Encoding _encoding;
        private string _userAgent;
        private string _dataBoundary;
        private string _contentType;
        private bool _encrypt;
        private string _encryptKey;
        
        public Encoding Encoding { get => this._encoding; set => this._encoding = value; }
        public string UserAgent { get => this._userAgent; set => this._userAgent = value; }
        public string DataBoundary { get => this._dataBoundary; set => this._dataBoundary = value; }
        public string ContentType { get => this._contentType; set => this._contentType = value; }
        public bool Encrypt { get => this._encrypt; set => this._encrypt = value; }
        public string EncryptKey { get => this._encryptKey; set => this._encryptKey = value; }

        public FileUpload()
        {
            ClearParameters();

            this._memonyPostParameters = new Dictionary<string, object>();
            this._encoding = Encoding.UTF8;
            this._userAgent = "User-Agent";
            this._dataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            this._contentType = "multipart/form-data";
            this._encrypt = false;
        }
                

        public Dictionary<string, string> SendPage(string url)
        {
            return this.SendPage(url, "");
        }


        public Dictionary<string, string> SendPage(string url, string userAgent)
        {
            if (userAgent.Equals(""))
                userAgent = this._userAgent;

            try
            {
                HttpWebResponse webResponse = this.MultipartFormDataPost(url, userAgent);

                if (HttpStatusCode.OK != webResponse.StatusCode)
                    throw new Exception("webResponse.StatusCode is not OK : " + webResponse.StatusCode);
                
                Stream dataStream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string rte = reader.ReadToEnd();

                int found = rte.IndexOf('}');

                if (found > 0)
                    rte = rte.Substring(0, found + 1);

                webResponse.Close();

                return new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(rte);
               
            }
            catch(WebException we)
            {
                string serverException = "";

                using (WebResponse response = we.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;

                    if (httpResponse == null)
                        throw we;

                    using(Stream sdata = response.GetResponseStream())
                    {
                        serverException = new StreamReader(sdata).ReadToEnd();
                    }
                }

                if (!string.IsNullOrEmpty(serverException))
                    throw new Exception(serverException);
                else
                    throw we;
            }
        }


        public HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent)
        {
            return this.MultipartFormDataPost(postUrl, userAgent, "");
        }


        public HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, string contentType)
        {
            if (!(contentType.Equals("")))
                this._contentType = contentType;

            string contentTypeString = this._contentType;
            contentTypeString += "; boundary=" + this._dataBoundary; ;


            byte[] formData = GetMultipartFormData(this._dataBoundary);
            return PostForm(postUrl, userAgent, contentTypeString, formData);
        }


        private byte[] GetMultipartFormData(string boundary)
        {
            Stream formDataStream = new MemoryStream();
            bool needsCLRF = false;
            string footer = "\r\n--" + boundary + "--\r\n";

            foreach (var param in this._memonyPostParameters)
            {
                if (needsCLRF)
                    formDataStream.Write(this._encoding.GetBytes("\r\n"), 0, this._encoding.GetByteCount("\r\n"));
                
                needsCLRF = true;

                if(param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                                                    boundary, param.Key, fileToUpload.FileName ?? param.Key, fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(this._encoding.GetBytes(header), 0, this._encoding.GetByteCount(header));
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                                                    boundary, param.Key, param.Value);

                    formDataStream.Write(this._encoding.GetBytes(postData), 0, this._encoding.GetByteCount(postData));
                }                
            } // end - foreah

            formDataStream.Write(this._encoding.GetBytes(footer), 0, this._encoding.GetByteCount(footer));

            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);

            formDataStream.Close();

            return formData;

        } // end - GetMultipartFormData()


        private HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
                throw new NullReferenceException("Request is not a http request");

            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;
            // request.PreAuthenticate = true;
            // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            // request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes("username" + ":" + "password")));

            using (Stream requestStream = request.GetRequestStream()) {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }


        /*
         ****************************************************************************************** 
         * Add Parameter Function Start
         * - 외부에서 접근하여 파라미터 추가
         * - String, File
         ******************************************************************************************
         */

        public string AddStringParam(string paramName, string value)
        {
            return AddStringParam(this._encrypt, paramName, value);
        }


        public string AddStringParam(bool encrypt, string paramName, string value)
        {
            string result;

            if (encrypt)
                result = AES.EncryptStringToString(value, this._encryptKey);
            else
                result = value;

            this._memonyPostParameters.Add(paramName, result);

            return result;
        }


        public void AddFileParam(string fileFullPath)
        {
            AddFileParam(Path.GetFileNameWithoutExtension(Path.GetFileName(fileFullPath))
                        , fileFullPath);
        }


        public void AddFileParam(string paramName, string fileFullPath)
        {
            AddFileParam(this._encrypt, paramName, fileFullPath);
        }


        public void AddFileParam(bool encrypt, string paramName, string fileFullPath)
        {
            string directoryName = Path.GetDirectoryName(fileFullPath);
            string extName = Path.GetExtension(fileFullPath);
            string fileName = paramName + extName;

            FileStream fileStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fileStream.Length];

            fileStream.Read(data, 0, data.Length);
            fileStream.Close();

            if (encrypt)
            {
                byte[] encrypted = AES.EncryptByteToByte(data, this._encryptKey);
                this._memonyPostParameters.Add(paramName, new FileParameter(encrypted, fileName, "application/octet-stream"));
            }
            else
            {
                this._memonyPostParameters.Add(paramName, new FileParameter(data, fileName, "application/octet-stream"));
            }
        }
        /*
         ****************************************************************************************** 
         * Add Parameter Function End
         ******************************************************************************************
         */

        public void ClearParameters()
        {
            this._memonyPostParameters.Clear();
        }


        private void DoWithResponse(HttpWebRequest request, Action<HttpWebResponse> responseAction)
        {
            Action wrapperAction = () =>
            {
                request.BeginGetResponse(new AsyncCallback((iar) =>
                {
                    var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                    responseAction(response);
                }), request);
            };

            wrapperAction.BeginInvoke(new AsyncCallback((iar) =>
            {
                var action = (Action)iar.AsyncState;
                action.EndInvoke(iar);
            }), wrapperAction);
        }


        private HttpWebResponse PostFormAsync(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
                throw new NullReferenceException("request is not a http request");

            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            DoWithResponse(request, (response) =>
            {
                var body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                Console.Write(body);
            });
            
            /*
            request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            request.Headers.Add("Authorization", "Basic" + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes("username" + ":" + "password")));
            */

            /*
            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }
            */
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Sub Class : FileParameter
        /// </summary>
        private class FileParameter
        {
            private byte[] _file;
            private string _fileName;
            private string _contentType;
            
            public byte[] File { get => this._file; set => this._file = value; }
            public string FileName { get => this._fileName; set => this._fileName = value; }
            public string ContentType { get => this._contentType; set => this._contentType = value; }
            
            public FileParameter(byte[] file): this(file, null) { }
            public FileParameter(byte[] file, string fileName): this(file, fileName, "") { }
            public FileParameter(byte[] file, string fileName, string contentType)
            {
                this._file = file;
                this._fileName = fileName;
                this._contentType = contentType;
            }
        }

    }
}
