namespace BusinessLogic.Contracts
{
    public class ChangePasswordDTO
    {
        public Guid Id { get; set; }
        public string CurrentPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }
}
