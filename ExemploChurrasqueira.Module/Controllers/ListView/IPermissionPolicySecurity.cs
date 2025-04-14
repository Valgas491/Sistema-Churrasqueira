
namespace ExemploChurrasqueira.Module.Controllers.ListView
{
    internal interface IPermissionPolicySecurity
    {
        void RunWithElevatedPermissions(Action value);
    }
}