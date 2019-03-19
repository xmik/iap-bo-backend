using System;

namespace BranchOfficeBackend
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return String.Format("Id: {0}, Name: {1}",
                this.ProjectId, this.Name);
        }
    }
}