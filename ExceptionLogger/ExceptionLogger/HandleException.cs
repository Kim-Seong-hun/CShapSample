using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace ExceptionLogger
{
    class HandleException
    {
        
        /// <summary>
        /// Exception 을 받아서 문자열 변환 및 파일기록
        /// </summary>        
        public string WriteException(Exception exception)
        {
            if (exception == null)
                return null;

            if (exception.GetBaseException().GetType() == typeof(ThreadAbortException))
                return null;

            return this.GetExceptionPhrase(exception);
        }

        /// <summary>
        /// Exception 과 BaseException(모두) 문자열로 리턴
        /// </summary>        
        private string GetExceptionPhrase(Exception exception)
        {
            Exception baseException = exception;
            StringBuilder returnString = new StringBuilder();
            int baseCount = 0;

            returnString.Append("* Generate Exception *");
            returnString.Append("\r\n\r\n");
            returnString.Append(this.MakeExceptionContent(exception));
            returnString.Append("\r\n\r\n");

            while ((baseException = baseException.GetBaseException()) != null)
            {
                baseCount++;
                returnString.Append(new String('<', baseCount));
                returnString.Append(" Base Exception ");
                returnString.Append(new String('>', baseCount));
                returnString.Append("\r\n\r\n");
                returnString.Append(this.MakeExceptionContent(baseException));
            }

            return returnString.ToString();
        }
        
        /// <summary>
        /// Exception 정보 문자열 리턴
        /// </summary>        
        private string MakeExceptionContent(Exception exception)
        {
            StringBuilder returnString = new StringBuilder();
            string stackFrameName = "";
            StackFrame stackFrame = new StackFrame(3, true);
            StackFrame[] stackFrames = new StackTrace().GetFrames();

            stackFrameName = stackFrame.GetMethod() + "(" + stackFrame.GetFileLineNumber() + ")";

            returnString.Append("- StackFrame : \r\n");
            returnString.Append(stackFrame);
            returnString.Append("- Name : \r\n");
            returnString.Append(exception.GetType().UnderlyingSystemType.FullName + "\r\n\r\n");
            returnString.Append("- Message : \r\n");
            returnString.Append(exception.Message + "\r\n\r\n");
            returnString.Append("- Message : \r\n");

            if (exception.TargetSite != null)
            {
                returnString.Append("- Target Site : \r\n");
                returnString.Append(exception.TargetSite.DeclaringType.FullName.ToString() + "\r\n");
                returnString.Append(exception.TargetSite.ToString() + "\r\n");
            }

            returnString.Append("- Stack Trace : \r\n");
            returnString.Append(exception.StackTrace);
            returnString.Append("\r\n\r\n");

            returnString.Append("- Stack Frames : \r\n");
            for (int i = 0; i < stackFrames.Length; i++)
                returnString.Append(stackFrames[i].GetMethod().Name + "\r\n");

            return returnString.ToString();
        } 

    }
}
