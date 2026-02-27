namespace eTeller.Domain.Exceptions
{
    public class PasswordNotValidException : Exception
    {
        public PasswordNotValidException() 
            : base("Password is not valid. Password has been used recently.")
        {
        }

        public PasswordNotValidException(string message) 
            : base(message)
        {
        }

        public PasswordNotValidException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    public class PasswordTooShortException : Exception
    {
        public PasswordTooShortException(int minimumLength) 
            : base($"Password must be at least {minimumLength} characters long.")
        {
        }

        public PasswordTooShortException(string message) 
            : base(message)
        {
        }

        public PasswordTooShortException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    public class PasswordMinDigitException : Exception
    {
        public PasswordMinDigitException(int minimumDigits) 
            : base($"Password must contain at least {minimumDigits} digits.")
        {
        }

        public PasswordMinDigitException(string message) 
            : base(message)
        {
        }

        public PasswordMinDigitException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    public class UserBlockedException : Exception
    {
        public string UserId { get; }

        public UserBlockedException(string userId) 
            : base($"User '{userId}' has been blocked due to too many failed login attempts.")
        {
            UserId = userId;
        }

        public UserBlockedException(string userId, string message) 
            : base(message)
        {
            UserId = userId;
        }

        public UserBlockedException(string userId, string message, Exception innerException) 
            : base(message, innerException)
        {
            UserId = userId;
        }
    }

    public class UserNotFoundException : Exception
    {
        public string UserId { get; }

        public UserNotFoundException(string userId) 
            : base($"User '{userId}' was not found.")
        {
            UserId = userId;
        }

        public UserNotFoundException(string userId, string message) 
            : base(message)
        {
            UserId = userId;
        }

        public UserNotFoundException(string userId, string message, Exception innerException) 
            : base(message, innerException)
        {
            UserId = userId;
        }
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() 
            : base("Invalid username or password.")
        {
        }

        public InvalidCredentialsException(string message) 
            : base(message)
        {
        }

        public InvalidCredentialsException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
