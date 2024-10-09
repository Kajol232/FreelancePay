namespace FreelancePay.Contract.Service
{
    public interface IAuthService
    {
        Task<bool> LoginUserAsync(string username, string password);
        Task LogoutUserAsync();
    }
}
