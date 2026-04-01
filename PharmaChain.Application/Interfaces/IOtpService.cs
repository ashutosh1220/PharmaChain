namespace PharmaChain.Application.Interfaces
{
    public interface IOtpService
    {
        public void StoreOtp(string email, string otp, DateTime expiry);
        public bool TryValidateOtp(string email, string otp);
    }
}
