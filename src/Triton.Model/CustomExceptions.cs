using System;

namespace Triton.Model.CustomExceptions
{
    public class DataNotFoundException:Exception
    {
        private string err_msg;
        public DataNotFoundException(string message)
        {
            err_msg = message;
        }

        public override string Message
        {
            get
            {
                return err_msg;
            }

        }
    }


}
