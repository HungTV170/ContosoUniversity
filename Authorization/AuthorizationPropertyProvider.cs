namespace ContosoUniversity.Authorization
{
    public class AuthorizationPropertyProvider
    {

        // user ID from AspNetUser table.
        public string? OwnerID { get; set; }
        public ContactStatus Status { get; set; }
    }

    public enum ContactStatus
    {
        Submitted,
        Approved,
        Rejected
    }
}
