namespace BusinessLogic.Contracts
{
    #pragma warning disable CS8618 
    public class ChangePasswordDTO
    {
        public Guid Id { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
