

namespace LdapTemplate.Results
{
    public class Result<T>
    {
        public Result()
        {
        }

        public Result(bool isSuccess, ResultType resultType, string message)
            : this(isSuccess, resultType, message, default)
        {
        }

        public Result(bool isSuccess, ResultType resultType, string message, T data)
        {
            IsSuccess = isSuccess;
            ResultType = resultType;
            Message = message;
            Data = data;
        }

        public T Data { get; set; }

        public bool IsSuccess { get; set; }

        public ResultType ResultType { get; set; }

        public string Message { get; set; }
    }
}