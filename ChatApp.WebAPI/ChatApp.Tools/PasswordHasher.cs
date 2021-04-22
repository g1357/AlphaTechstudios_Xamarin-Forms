using System;
using System.Security.Cryptography;

/// <summary>
/// Инструменты приложения
/// </summary>
namespace ChatApp.Tools
{
    /// <summary>
    /// Хэшировщик пароля
    /// </summary>
    public static class PasswordHasher
    {
        // 24 = 192 bits
        private const int SaltByteSize = 24; // Размер модификатора пароля
        private const int HashByteSize = 24; // Размер хэша
        private const int HashingIterationsCount = 10101; // Счётчик итераций хэширования

        /// <summary>
        /// Вычислить хэш
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <param name="salt">Модификатор</param>
        /// <param name="iterations">Колическтво итераций</param>
        /// <param name="hashByteSize">размер хзша</param>
        /// <returns></returns>
        public static byte[] ComputeHash(string password, byte[] salt, int iterations = HashingIterationsCount, int hashByteSize = HashByteSize)
        {
            Rfc2898DeriveBytes hashGenerator = new Rfc2898DeriveBytes(password, salt);
            hashGenerator.IterationCount = iterations;
            return hashGenerator.GetBytes(hashByteSize);
        }

        /// <summary>
        /// Сгенерировать модификатор парол.
        /// </summary>
        /// <param name="saltByteSize">Размер модификатора в байтах</param>
        /// <returns>Модификатор пароля</returns>
        public static byte[] GenerateSalt(int saltByteSize = SaltByteSize)
        {
            RNGCryptoServiceProvider saltGenerator = new RNGCryptoServiceProvider();
            byte[] salt = new byte[saltByteSize];
            saltGenerator.GetBytes(salt);
            return salt;
        }

        /// <summary>
        /// Проверить пароль.
        /// Модификатор и хзш заданы маривами байтов.
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <param name="passwordSalt">Модификатор пароля</param>
        /// <param name="passwordHash">Хэш пароля</param>
        /// <returns></returns>
        private static bool VerifyPassword(string password, byte[] passwordSalt,
            byte[] passwordHash)
        {
            byte[] computedHash = ComputeHash(password, passwordSalt);
            return AreHashesEqual(computedHash, passwordHash);
        }

        /// <summary>
        /// Проверить пароль.
        /// Модификатор и хэш задат=ны текстовыми строками.
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <param name="salt">Модификатор пароля</param>
        /// <param name="oldPassword">Старый пароль</param>
        /// <returns></returns>
        public static bool VerifyPassword(string password, string salt, string oldPassword)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var oldPwdHash = Convert.FromBase64String(oldPassword);
            return VerifyPassword(password, saltBytes, oldPwdHash);
        }

        /// <summary>
        /// Проверить хэши на равенство.
        /// </summary>
        /// <param name="firstHash">Первый хэш/param>
        /// <param name="secondHash">Второй хэш</param>
        /// <returns></returns>
        //Length constant verification - prevents timing attack
        private static bool AreHashesEqual(byte[] firstHash, byte[] secondHash)
        {
            int minHashLenght = firstHash.Length <= secondHash.Length ? firstHash.Length : secondHash.Length;
            var xor = firstHash.Length ^ secondHash.Length;
            for (int i = 0; i < minHashLenght; i++)
                xor |= firstHash[i] ^ secondHash[i];
            return 0 == xor;
        }
    }
}
