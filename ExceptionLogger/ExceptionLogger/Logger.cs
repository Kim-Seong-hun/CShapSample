using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace ExceptionLogger
{
    class Logger
    {
   
        private string logFolder;       // 로그파일 저장 폴더
        private string logFileName;     // 로그파일명
        private Encoding logEncoding;   // 로그파일 저장 문자열 셋

        public string LogFoler
        {
            get { return this.logFolder; }
        }

        public string LogFileName
        {
            get { return this.logFileName; }
        }

        public Encoding LogEncoding
        {
            get { return this.logEncoding; }
        }


        public Logger()
        {
            this.SetLogFolderName("Log");
            this.SetLogFileName("");
            this.SetLogEncoding(Encoding.UTF8);
        }
        
        /// <summary>
        /// 로그 파일 저장 폴더명 수정
        /// </summary>
        /// <param name="name">수정할 폴더명</param>
        public void SetLogFolderName(string name)
        {
            this.logFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.logFolder += "\\" + name;
        }
       
        /// <summary>
        /// 저장할 로그 파일명 변경
        /// </summary>
        /// <param name="name"></param>
        public void SetLogFileName(string name)
        {
            this.logFileName = name;
        }

        /// <summary>
        /// 로그파일 Character Set
        /// </summary>
        /// <param name="encondingType"></param>
        public void SetLogEncoding(Encoding encondingType)
        {
            this.logEncoding = encondingType;
        }

        /// <summary>
        /// 로그 문자열을 처리하여 파일 기록 함수 호출
        /// </summary>
        public void WriteLogToFile(string logString)
        {
            logString = logString.Trim();

            if (logString == string.Empty)
                return;

            string[] lines = logString.Split(new string[] { Environment.NewLine }
                                            , StringSplitOptions.RemoveEmptyEntries);

            StringWriter sw = new StringWriter();
            sw.WriteLine(lines[0]);

            if(lines.Length > 1)
            {
                for (int i = 1; i < lines.Length; i++)
                    sw.WriteLine(new string(' ', 11) + lines[i]);
            }

            string resultValue = sw.ToString();

            if (resultValue.EndsWith(Environment.NewLine))
                resultValue = resultValue.Substring(0, resultValue.LastIndexOf(Environment.NewLine));

            this.WriteLogPlain(resultValue);
        }

        /// <summary>
        /// 인자로 받은 로그문자열을 파일에 추가
        /// </summary>
        /// <param name="logString"></param>
        private void WriteLogPlain(string logString)
        {
            if (!(Directory.Exists(this.logFolder)))
                Directory.CreateDirectory(this.logFolder);

            string logWriteString = GetTimeFormatString() + " ";
            logWriteString += logString;
            logWriteString += Environment.NewLine;

            File.AppendAllText(this.GetCurrentLogFileName(), logWriteString, this.logEncoding);
        }

        /// <summary>
        /// 로그 파일 이름 정의하여 리턴
        /// Type 1 : 파일명을 지정할 경우           : 파일명_yyyyMMdd.log
        /// Type 2 : 파일명을 지정하지 않을 경우    : yyyyMMdd.log
        /// </summary>
        private string GetCurrentLogFileName()
        {
            string resultValue = this.logFolder + "\\";

            if (!this.logFileName.Equals(""))
                resultValue += this.logFileName + "_";

            resultValue += DateTime.Now.ToString("yyyyMMdd") + ".log";

            return resultValue;
        }

        /// <summary>
        /// 로그에 사용할 시간 문자열 생성 및 리턴
        /// </summary>
        private string GetTimeFormatString()
        {
            string resultValue = "[";
            resultValue += DateTime.Now.Hour.ToString("00") + ":";
            resultValue += DateTime.Now.Minute.ToString("00") + ":";
            resultValue += DateTime.Now.Second.ToString("00") + "]";

            return resultValue;
        }

        /// <summary>
        /// 현재 날짜 - dayInterval 이전 로그 파일 삭제
        /// 삭제한 파일 개수 리턴
        /// </summary>        
        public int DeleteDayIntervalLogFiles(int dayInterval)
        {
            DateTime maginDate = DateTime.Now.AddDays(0 - (dayInterval));
            String logFileName;
            String temp;
            int outDate = 0;
            int deletCount = 0;

            if (!(Directory.Exists(this.logFolder)))
                return 0;

            foreach(string fileInfo in Directory.GetFiles(this.logFolder))
            {
                logFileName = Path.GetFileNameWithoutExtension(fileInfo);
                //logFileName = Regex.Replace(logFileName, @"\D", "");

                if (logFileName.IndexOf('_') == -1)
                    temp = logFileName;
                else
                {
                    string[] subStrings = Path.GetFileNameWithoutExtension(fileInfo).Split('_');
                    temp = subStrings[subStrings.Length - 1];
                }
                    
                if(int.TryParse(temp, out outDate))
                {
                    if (outDate < int.Parse(maginDate.Year.ToString("0000") + maginDate.Month.ToString("00") + maginDate.Day.ToString("0")))
                    {
                        File.Delete(fileInfo);
                        deletCount++;
                    }
                }
            }

            return deletCount;
        }

    }
}
