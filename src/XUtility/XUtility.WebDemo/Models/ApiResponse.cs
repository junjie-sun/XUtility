using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XUtility.WebDemo.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }

        public int Status { get; set; }

        public string Message { get; set; }
    }
}
