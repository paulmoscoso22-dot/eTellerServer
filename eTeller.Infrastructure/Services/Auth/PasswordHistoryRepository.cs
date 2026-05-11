using System.Security.Cryptography;
using System.Text;
using Dapper;
using eTeller.Application.Contracts.Auth;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Services.Auth;

/// <summary>
/// Implementazione Dapper di IPasswordHistoryRepository.
/// Gestisce lo storico password in sys_USER_PASSWD via stored procedure legacy.
/// </summary>
public class PasswordHistoryRepository : IPasswordHistoryRepository
{
    private readonly eTellerDbContext _context;

    public PasswordHistoryRepository(eTellerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<bool> IsPasswordInHistoryAsync(string userId, string plainPassword, int maxHistory)
    {
        // Recupero le ultime N password dallo storico
        const string sql = @"
            SELECT TOP (@MaxHistory) PWH_PASSW
            FROM sys_USER_PASSWD
            WHERE PWH_USER = @User
            ORDER BY PWH_PASSNUM DESC";

        using var connection = new SqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var storedHashes = await connection.QueryAsync<string>(
            sql, new { User = userId, MaxHistory = maxHistory });

        // Verifica BCrypt-first, MD5 fallback
        var md5Hash = ComputeMd5LowercaseHex(plainPassword);
        foreach (var stored in storedHashes)
        {
            if (string.IsNullOrWhiteSpace(stored)) continue;

            if (stored.StartsWith("$2", StringComparison.Ordinal))
            {
                if (BCrypt.Net.BCrypt.Verify(plainPassword, stored))
                    return true;
            }
            else
            {
                // MD5 legacy
                if (string.Equals(md5Hash, stored, StringComparison.Ordinal))
                    return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public async Task InsertPasswordAsync(string userId, string passwordHash, DateTime modDate)
    {
        using var connection = new SqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            "dbo.sys_USER_PASSWD_Insert_Password",
            new { User = userId, Password = passwordHash, ModDate = modDate },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    /// <inheritdoc />
    public async Task ClearOldPasswordsAsync(string userId, int maxHistory)
    {
        using var connection = new SqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            "dbo.sys_USER_PASSWD_Clear_Password_History",
            new { User = userId, MaxHistory = maxHistory },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    private static string ComputeMd5LowercaseHex(string input)
        => Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(input))).ToLower();
}
