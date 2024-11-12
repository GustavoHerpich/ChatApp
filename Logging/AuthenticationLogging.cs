namespace Chat.Logging;

public static partial class LoggingExtension
{
    [LoggerMessage(LogLevel.Information, "Generating token for user: {Username}.")]
    public static partial void StartingToGenerateToken(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Information, "Attempting to authenticate user: {Username}.")]
    public static partial void AuthenticateAttempt(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Warning, "User {Username} not validated.")]
    public static partial void UserNotValidated(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Information, "User {Username} successfully validated.")]
    public static partial void UserValidated(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Error, "Failed to generate token for user {Username}.")]
    public static partial void TokenGenerationFailed(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Information, "Token successfully generated for user: {Username}, Token: {Token}.")]
    public static partial void TokenGenerated(
        this ILogger logger,
        string username,
        string token);

    [LoggerMessage(LogLevel.Information, "User {Username} successfully authenticated in our system.")]
    public static partial void UserAuthenticated(
       this ILogger logger,
       string username);

    [LoggerMessage(LogLevel.Information, "Attempting to register new user: {Username}.")]
    public static partial void RegisterAttempt(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Warning, "User {Username} already exists.")]
    public static partial void UserAlreadyExists(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Information, "Creating user {Username}.")]
    public static partial void CreatingUser(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Information, "User {Username} successfully created.")]
    public static partial void UserCreated(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Information, "Attempting to recover password for user: {Username}.")]
    public static partial void RecoverPasswordAttempt(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Warning, "User {Username} not found.")]
    public static partial void UserNotFound(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Warning, "The new password matches the current password for user {Username}.")]
    public static partial void PasswordsMatch(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Information, "Updating password for user {Username}.")]
    public static partial void UpdatingPassword(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Information, "Password updated successfully for user {Username}.")]
    public static partial void PasswordUpdated(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Warning, "Missing necessary arguments for user validation. Username: {Username}.")]
    public static partial void MissingValidationArguments(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Warning, "Incorrect password provided for user {Username}.")]
    public static partial void InvalidPassword(
        this ILogger logger,
        string username);

    [LoggerMessage(LogLevel.Warning, "Password for user {Username} has expired.")]
    public static partial void PasswordExpired(
        this ILogger logger,
        string username);
}

