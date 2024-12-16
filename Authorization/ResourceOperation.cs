using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ContosoUniversity.Authorization{
    public static class ResourceOperation{
        public static OperationAuthorizationRequirement Create =   
          new OperationAuthorizationRequirement {Name=ContosoResource.CreateOperationName};
        public static OperationAuthorizationRequirement Read = 
          new OperationAuthorizationRequirement {Name=ContosoResource.ReadOperationName};  
        public static OperationAuthorizationRequirement Update = 
          new OperationAuthorizationRequirement {Name=ContosoResource.UpdateOperationName}; 
        public static OperationAuthorizationRequirement Delete = 
          new OperationAuthorizationRequirement {Name=ContosoResource.DeleteOperationName};
        public static OperationAuthorizationRequirement Approve = 
          new OperationAuthorizationRequirement {Name=ContosoResource.ApproveOperationName};
        public static OperationAuthorizationRequirement Reject = 
          new OperationAuthorizationRequirement {Name=ContosoResource.RejectOperationName};       
    }

        public class ContosoResource
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";

        public static readonly string ContosoAdministratorsRole = 
                                                              "ContactAdministrators";
        public static readonly string ContosoManagersRole = "ContactManagers";
    }
}