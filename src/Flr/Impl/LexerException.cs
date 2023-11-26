namespace Flr.Impl;

internal class LexerException(string message, Exception innerException) : Exception(message, innerException);
