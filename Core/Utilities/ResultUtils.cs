using Compendium.Results;

using System;

namespace Compendium.Utilities
{
    public static class ResultUtils
    {
        public static readonly IResult[] EmptyResults = new IResult[0];

        public static IResult Error()
            => new ErrorResult(null, null);

        public static IResult Error(string message)
            => new ErrorResult(message, null);

        public static IResult Error(Exception exception)
            => new ErrorResult(exception);

        public static IResult Error(string message, Exception exception)
            => new ErrorResult(message, exception);

        public static IResult Success(object result)
            => new SuccessResult(result);

        public static IResult Success()
            => new SuccessResult(null);

        public static IResult Copy(this IResult result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            if (result is ErrorResult errorResult)
                return new ErrorResult(errorResult.Message, errorResult.Exception);

            if (result is SuccessResult successResult)
                return new SuccessResult(successResult.Result);

            return result;
        }

        public static string ReadErrorMessage(this IResult result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            if (result.IsSuccess)
                throw new ArgumentException($"Attempted to read error message of a success result.");

            if (result is not ErrorResult errorResult)
                throw new ArgumentException($"The provided result type is not of ErrorResult.");

            return errorResult.Message;
        }

        public static Exception ReadException(this IResult result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            if (result.IsSuccess)
                throw new ArgumentException($"Attempted to read error message of a success result.");

            if (result is not ErrorResult errorResult)
                throw new ArgumentException($"The provided result type is not of ErrorResult.");

            return errorResult.Exception;
        }

        public static bool TryReadValue(this IResult result, out object value)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            if (!result.IsSuccess)
            {
                value = null;
                return false;
            }

            value = result.Result;
            return true;
        }

        public static bool TryReadValue<TValue>(this IResult result, out TValue value)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            if (!TryReadValue(result, out var objectResult))
            {
                value = default;
                return false;
            }    

            if (objectResult is not TValue boxedValue)
            {
                value = default;
                return false;
            }

            value = boxedValue;
            return true;
        }
    }
}
