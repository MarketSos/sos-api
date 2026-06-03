namespace Sos.Shared.Kernel.Results;

/// <summary>
/// Результат операции — успех или ошибка.
/// Используется вместо исключений для предсказуемого потока управления.
/// </summary>
public class Result
{
    /// <summary>
    /// Операция выполнена успешно
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Сообщение об ошибке (null при успехе)
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Операция завершилась ошибкой
    /// </summary>
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Создать успешный результат
    /// </summary>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Создать результат с ошибкой
    /// </summary>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Создать успешный результат с данными
    /// </summary>
    public static Result<T> Success<T>(T value) => new(value, true, null);

    /// <summary>
    /// Создать результат с ошибкой (с типом данных)
    /// </summary>
    public static Result<T> Failure<T>(string error) => new(default!, false, error);
}

/// <summary>
/// Результат операции с возвращаемым значением.
/// </summary>
public class Result<T> : Result
{
    /// <summary>
    /// Возвращаемое значение (только при IsSuccess = true)
    /// </summary>
    public T Value { get; }

    internal Result(T value, bool isSuccess, string? error) : base(isSuccess, error)
    {
        Value = value;
    }
}
