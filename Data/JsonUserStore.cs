using ContosoUniversity.Data;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace ContosoUniversity.Data
{
    public class JsonUserStore : IUserStore<ContosoUser>, IUserPasswordStore<ContosoUser>, IUserEmailStore<ContosoUser>
    {
        private readonly string _filePath = "data.json";
        private DataStore _dataStore =new();

        public JsonUserStore()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(new DataStore()));
            }
            LoadData();
        }

        private void LoadData()
        {
            var json = File.ReadAllText(_filePath);
            _dataStore = JsonConvert.DeserializeObject<DataStore>(json) ?? new DataStore();
        }

        private void SaveData()
        {
            var json = JsonConvert.SerializeObject(_dataStore, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public Task<IdentityResult> CreateAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            if (_dataStore.Users.Any(u => u.UserName == user.UserName))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "User already exists" }));
            }

            _dataStore.Users.Add(user);
            SaveData();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            var index = _dataStore.Users.FindIndex(u => u.Id == user.Id);

            if (index >= 0)
            {
                _dataStore.Users[index] = user;
                SaveData();
                return Task.FromResult(IdentityResult.Success);
            }

            return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "User not found" }));
        }

        public Task<IdentityResult> DeleteAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            var userToRemove = _dataStore.Users.FirstOrDefault(u => u.Id == user.Id);

            if (userToRemove != null)
            {
                _dataStore.Users.Remove(userToRemove);
                SaveData();
                return Task.FromResult(IdentityResult.Success);
            }

            return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "User not found" }));
        }

        public Task<ContosoUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_dataStore.Users.FirstOrDefault(u => u.Id == userId));
        }

        public Task<ContosoUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_dataStore.Users.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName));
        }

        public Task<string?> GetPasswordHashAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(ContosoUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            SaveData();
            return Task.CompletedTask;
        }

        public Task<string?> GetUserNameAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(ContosoUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName ?? throw new ArgumentNullException();
            SaveData();
            return Task.CompletedTask;
        }

        public Task<string> GetUserIdAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string?> GetNormalizedUserNameAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(ContosoUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            SaveData();
            return Task.CompletedTask;
        }

        public Task SetEmailAsync(ContosoUser user, string? email, CancellationToken cancellationToken)
        {
            user.Email = email ?? throw new ArgumentNullException(nameof(email));
            SaveData();
            return Task.CompletedTask;
        }

        public Task<string?> GetEmailAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ContosoUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            SaveData();
            return Task.CompletedTask;
        }

        public Task<ContosoUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var user = _dataStore.Users.FirstOrDefault(u => u.NormalizedEmail == normalizedEmail);
            return Task.FromResult(user);
        }

        public Task<string?> GetNormalizedEmailAsync(ContosoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ContosoUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            SaveData();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // No resources to dispose.
        }
    }

}
