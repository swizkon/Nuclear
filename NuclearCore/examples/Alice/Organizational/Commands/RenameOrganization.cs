using Alice.Organizational.Domain;
using Nuclear.NetCore.Commands;

namespace Alice.Organizational.Commands
{
    public class RenameOrganization : IAggregateCommand<Organization>
    {
        public string NewName { get; set; }
    }
}