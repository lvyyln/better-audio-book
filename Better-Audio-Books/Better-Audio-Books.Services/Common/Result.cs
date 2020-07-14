using System;

namespace Better_Audio_Books.Services.Common
{
    public struct Result
    {
        private readonly ResultCommonLogic _logic;
        public bool IsFailure => _logic.IsFailure;
        public bool IsSuccess => _logic.IsSuccess;
        public string Error => _logic.Error;

        private Result(bool isFailure, string error)
        {
            _logic = new ResultCommonLogic(isFailure, error);
        }

        public static Result Success()
        {
            return new Result(false, string.Empty);
        }

        public static Result<T> Success<T>(T value)
        {
            return new Result<T>(false, string.Empty, value);
        }

        public static Result Failure(string error)
        {
            return new Result(true, error);
        }

        public static Result<T> Failure<T>(string error)
        {
            return new Result<T>(true, error);
        }
    }

    public partial struct Result<T>
    {
        private readonly ResultCommonLogic _logic;
        public bool IsFailure => _logic.IsFailure;
        public bool IsSuccess => _logic.IsSuccess;
        public string Error => _logic.Error;

        private readonly T _value;
        public T Value => IsSuccess ? _value : throw new InvalidOperationException(Error);

        internal Result(bool isFailure, string error, T value)
        {
            _logic = new ResultCommonLogic(isFailure, error);
            _value = value;
        }

        internal Result(bool isFailure, string error) : this()
        {
            _logic = new ResultCommonLogic(isFailure, error);
        }

        public static implicit operator Result<T>(T value)
        {
            if (value is T result)
                return result;

            return Result.Success(value);
        }

        public static implicit operator Result(Result<T> result)
        {
            if (result.IsSuccess)
                return Result.Success();
            else
                return Result.Failure(result.Error);
        }
    }

    internal struct ResultCommonLogic
    {
        private readonly string _error;
        public bool IsFailure { get; }
        public bool IsSuccess => !IsFailure;
        public string Error => IsFailure ? _error : throw new InvalidOperationException();

        public ResultCommonLogic(bool isFailure, string error)
        {
            if (isFailure)
            {
                if (error == null || (error != null && error.Equals(string.Empty)))
                    throw new ArgumentNullException(nameof(error), "Error Object Is Not Provided For Failure");
            }
            else
            {
                if (!string.Equals(error, string.Empty))
                    throw new ArgumentException("Error Object Is Provide For Success");
            }

            IsFailure = isFailure;
            _error = error;
        }
    }
}