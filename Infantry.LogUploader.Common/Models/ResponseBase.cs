namespace Infantry.LogUploader.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    public class ResponseBase
    {
        public IEnumerable<string> Errors { get; set; }

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
