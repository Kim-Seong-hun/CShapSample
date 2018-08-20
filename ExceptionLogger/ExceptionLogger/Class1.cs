using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionLogger
{
    public class Class1
    {
        Logger logger;
        HandleException handleException;

        public Class1()
        {
            logger = new Logger();
            handleException = new HandleException();
        }

        public void HappenException(Exception exception)
        {
            string exceptionPhrase = handleException.WriteException(exception);

            if (exceptionPhrase == null)
                return;
                
            logger.WriteLogToFile("[Exception]");
            logger.WriteLogToFile(exceptionPhrase);
        }

        public void ChangeLogFolderName(string name)
        {
            logger.SetLogFolderName(name);
        }

        public void ChangeLogFileName(string name)
        {
            logger.SetLogFileName(name);
        }

        public void ChangeLogEncoding(string encodingName)
        {
            encodingName = encodingName.ToLower();

            if (encodingName.Equals("ascii"))
                logger.SetLogEncoding(Encoding.ASCII);
            else if (encodingName.Equals("utf8"))
                logger.SetLogEncoding(Encoding.UTF8);

        }
    }
}
