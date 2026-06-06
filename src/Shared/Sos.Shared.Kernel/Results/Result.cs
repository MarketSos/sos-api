namespace Sos.Shared.Kernel.Results;

public class Result
{
    public bool    IsSuccess { get; }
    public string? Error     { get; }
    public bool    IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error     = error;
    }

    // ── Muvaffaqiyat ──────────────────────────────────────────────────────────
    public static Result    Success()        => new(true, null);
    public static Result<T> Success<T>(T v)  => new(v, true, null);

    // ── Umumiy xato ───────────────────────────────────────────────────────────
    public static Result    Failure(string error)        => new(false, error);
    public static Result<T> Failure<T>(string error)     => new(default!, false, error);

    // ── Topilmadi — entity nomi va qidiruv qiymati avtomatik kiritiladi ────────
    // "EntityName [key] topilmadi."
    public static Result NotFound<TEntity>(object key)
        => Failure($"{typeof(TEntity).Name} [{key}] topilmadi.");

    public static Result<T> NotFound<T, TEntity>(object key)
        => Failure<T>($"{typeof(TEntity).Name} [{key}] topilmadi.");

    // Generic bo'lmagan entitylar uchun (masalan IdentityRole<Guid>)
    public static Result NotFound(string entityName, object key)
        => Failure($"{entityName} [{key}] topilmadi.");

    public static Result<T> NotFound<T>(string entityName, object key)
        => Failure<T>($"{entityName} [{key}] topilmadi.");

    // ── Allaqachon mavjud — entity nomi va kalit qiymati avtomatik kiritiladi ─
    // "EntityName [key] allaqachon mavjud."
    public static Result Conflict<TEntity>(object key)
        => Failure($"{typeof(TEntity).Name} [{key}] allaqachon mavjud.");

    public static Result<T> Conflict<T, TEntity>(object key)
        => Failure<T>($"{typeof(TEntity).Name} [{key}] allaqachon mavjud.");

    public static Result Conflict(string entityName, object key)
        => Failure($"{entityName} [{key}] allaqachon mavjud.");

    public static Result<T> Conflict<T>(string entityName, object key)
        => Failure<T>($"{entityName} [{key}] allaqachon mavjud.");
}

public class Result<T> : Result
{
    public T Value { get; }

    internal Result(T value, bool isSuccess, string? error) : base(isSuccess, error)
    {
        Value = value;
    }
}
