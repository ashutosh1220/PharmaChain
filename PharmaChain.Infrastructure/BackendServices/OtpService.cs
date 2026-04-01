using PharmaChain.Application.Interfaces;

public class OtpService : IOtpService
{
    private readonly Dictionary<string, (string otp, DateTime expiry)> _store = new();

    public void StoreOtp(string email, string otp, DateTime expiry)
    {
        _store[email] = (otp, expiry);
    }

    public bool TryValidateOtp(string email, string otp)
    {
        if (_store.TryGetValue(email, out var entry))
        {
            if (entry.otp == otp && DateTime.UtcNow <= entry.expiry)
            {
                _store.Remove(email);
                return true;
            }
        }
        return false;
    }
}