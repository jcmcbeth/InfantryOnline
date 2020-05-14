namespace Infantry.LogUploader.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    public class ResponseBase
    {
        public List<string> Errors { get; set; }

        public static ResponseBase CreateErrorResponse(string error)
        {
            return new ResponseBase()
            {
                Errors = new List<string>()
                {
                    error
                }
            };
        }
    }
}
