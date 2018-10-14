using Nuclear.NetCore.Commands;

namespace Alice.Organizational.Commands
{
    public class CreateOrganization : ICommand
    {
        public string Name { get; set; }
    }
}