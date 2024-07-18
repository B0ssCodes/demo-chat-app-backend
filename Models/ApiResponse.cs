using System.Net;

namespace ChatApp.Models
{
    public class ApiResponse<T>
    {
        public HttpStatusCode Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }



    }
}
