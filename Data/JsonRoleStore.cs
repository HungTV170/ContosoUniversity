using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace ContosoUniversity.Data
{
    public class JsonRoleStore : IRoleStore<IdentityRole>
    {
        private readonly string _filePath = "data.json";

        private DataStore _dataStore =new();

        public JsonRoleStore()
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

        public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            if (_dataStore.Roles.Any(r => r.Name == role.Name))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Role already exists" }));
            }

            _dataStore.Roles.Add(role);
            SaveData();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            _dataStore.Roles.Remove(role);
            SaveData();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_dataStore.Roles.FirstOrDefault(r => r.Id == roleId));
        }

        public Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_dataStore.Roles.FirstOrDefault(r => r.Name?.ToLower() == normalizedRoleName.ToLower()));
        }

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName ?? throw new ArgumentNullException();
            SaveData();
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
