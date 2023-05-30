using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        //IConfigure is used to log the error
        //IHostEnvironment is used to check
        //whether we are in dev or prod mode
        public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger,IHostEnvironment env)
        {

            //next is basically used to say what is the next middleware
            //logger is used to log the exception to the console
            //env to check whether we are in dev mode or prod mode
            _env = env;
            _logger = logger;
            _next = next;

        }


        //Httpcontext holds all the http request and response data
        //this method has to be called
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //basically it waits for request to throw an exception
                //if it is not handled at the controller level or 
                //at any other level
                //it will be handled here
                await _next(context);
            }
            catch(Exception ex){
                //logs the exception message
                 _logger.LogError(ex,ex. Message);
                //when we handle it in api controller 
                //it will be done by default
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                

                //if we are in dev mode we will display the stack trace
                //main reason for converting it to string is
                //cause of \n(new line) which is present inside the stack trace
                //else just a generic message
                var response = _env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new ApiException(context.Response.StatusCode, ex.Message,"Internal Server Error");

                //returning in json format
                //it has to follow camel case
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response, options); 


               
                //json response will be returned
                await context.Response.WriteAsync(json);
                
            }
        }
    }
}